﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WebExtensionPack
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [Guid(Vsix.Id)]
    public sealed class VSPackage : Package
    {
        protected async override void Initialize()
        {
            Logger.Initialize(this, Vsix.Name);
            Telemetry.Initialize(this, Vsix.Version, "fbfac2d0-cd41-4458-9106-488be47240c2");

            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(async () =>
            {
                await Install();

            }), DispatcherPriority.SystemIdle, null);

            base.Initialize();
        }

        private async System.Threading.Tasks.Task Install()
        {
            var repository = (IVsExtensionRepository)GetService(typeof(SVsExtensionRepository));
            var manager = (IVsExtensionManager)GetService(typeof(SVsExtensionManager));

            var installed = manager.GetInstalledExtensions();
            var products = ExtensionList.Products();
            var missing = products.Where(product => !installed.Any(ins => ins.Header.Identifier == product.Key));

            if (!missing.Any())
                return;

            var dte = (EnvDTE.DTE)GetService(typeof(EnvDTE.DTE));

            var hwnd = new IntPtr(dte.MainWindow.HWnd);
            var window = (Window)System.Windows.Interop.HwndSource.FromHwnd(hwnd).RootVisual;

            var dialog = new InstallerProgress(missing, $"Downloading extensions...");
            dialog.Owner = window;
            dialog.Show();

            await System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var product in missing)
                {
                    if (!dialog.IsVisible)
                        break; // User cancelled the dialog

                    dialog.StartDownloading(product.Key);
                    dialog.SetMessage($"Installing {product.Value}...");
                    InstallExtension(repository, manager, product);
                    dialog.InstallComplete(product.Key);
                }
            });

            if (dialog.IsVisible)
            {
                dialog.Close();
                dialog = null;
                PromptForRestart(missing.Select(ext => ext.Value));
            }
        }

        private void InstallExtension(IVsExtensionRepository repository, IVsExtensionManager manager, KeyValuePair<string, string> product)
        {
#if DEBUG
            System.Threading.Thread.Sleep(1000);
            return;
#endif

            try
            {
                GalleryEntry entry = repository.CreateQuery<GalleryEntry>(includeTypeInQuery: false, includeSkuInQuery: true, searchSource: "ExtensionManagerUpdate")
                                                                                 .Where(e => e.VsixID == product.Key)
                                                                                 .AsEnumerable()
                                                                                 .FirstOrDefault();

                if (entry != null)
                {
                    IInstallableExtension installable = repository.Download(entry);
                    manager.Install(installable, false);

                    Telemetry.TrackEvent(installable.Header.Name);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void PromptForRestart(IEnumerable<string> extensions)
        {
            string list = string.Join(Environment.NewLine, extensions);
            string prompt = $"The following extensions were installed:\r\r{list}\r\rDo you want to restart Visual Studio now?";
            var result = MessageBox.Show(prompt, Vsix.Name, MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                IVsShell4 shell = (IVsShell4)GetService(typeof(SVsShell));
                shell.Restart((uint)__VSRESTARTTYPE.RESTART_Normal);
            }
        }
    }
}
