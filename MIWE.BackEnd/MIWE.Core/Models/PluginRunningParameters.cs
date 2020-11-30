using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MIWE.Core.Models
{
    public class PluginRunningParameters
    {

        public string CrawlerPluginPath { get; set; }

        public string ProcessorPluginPath { get; set; }

        public string MerchantName { get; set; }

        /// <summary>
        /// Gets or sets the processor save action. First parameter contains the data to be saved. Second paramter is the extension type 
        /// of the file to be saved to.
        /// </summary>
        /// <value>
        /// The processor save action.
        /// </value>
        public Action<MemoryStream, string> ProcessorSaveAction { get; set; }

        public bool IsProcessorAssigned()
        {
            return !string.IsNullOrEmpty(ProcessorPluginPath) && !string.IsNullOrEmpty(MerchantName);
        }
    }
}
