using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scaneva.Core
{
    public class ScanevaCoreSettings
    {
        public string HWSettingsFilePath { get; set; } = "HardwareSettings.xml";
        public string DefaultScanMethodDirectory { get; set; } = ".\\";
        public string ScanResultDirectory { get; set; } = ".\\Results";
        public string PositionStoreFilePath { get; set; } = "PositionStore.xml";
        public string LogDirectory { get; set; } = ".\\Log";
    }
}
