﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anvil.Models
{
    public class WalletImport
    {
        public string Mnemonic { get; set; }

        public bool SaveMnemonic { get; set; }

        public string Password { get; set; }

        public string MnemonicStorePath { get; set; }

        public string PrivateKeyFilePath { get; set; }
    }
}
