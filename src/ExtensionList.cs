﻿using System.Collections.Generic;

namespace WebExtensionPack
{
    class ExtensionList
    {
        public static IDictionary<string, string> Products()
        {
            return new Dictionary<string, string> {
                { "5fb7364d-2e8c-44a4-95eb-2a382e30fec9", "Web Essentials" },
                { "148ffa77-d70a-407f-892b-9ee542346862", "Web Compiler"},
                { "36bf2130-106e-40f2-89ff-a2bdac6be879", "Web Analyzer"},
                { "bf95754f-93d3-42ff-bfe3-e05d23188b08", "Image optimizer"},
                { "950d05f7-bb25-43ce-b682-44b377b5307d", "Glyphfriend"},
                { "f4ab1e64-5d35-4f06-bad9-bf414f4b3bbb", "Open Command Line"},
                { "fdd64809-376e-4542-92ce-808a8df06bcc", "Package Installer"},
                { "10d9b3af-1338-4c45-bc99-4ec38c3a11fb", "Browser Sync"},
                { "2d8aa02a-8810-421f-97b9-86efc573fea3", "Browser Reload on Save"},
                { "2a20580c-7be4-4440-bcd6-8dcf5aa2004e", "JavaScript Snippet Pack" },
                { "51b81721-cf4e-4ce0-a595-972b1ca2a186", "Suggested Extensions" },
                { "2E78AA18-E864-4FBB-B8C8-6186FC865DB3", "Add New File" },
                { "25a79d25-0fff-4748-afaa-3a67ed116bc9", "Web Accessibility Checker" },
            };
        }
    }
}
