using Anvil.Core.ViewModels;
using Anvil.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anvil.ViewModels.NFTs
{
    public class NFTViewModel : ViewModelBase
    {
        public string Header => "Find Holders";

        private string _searchString = string.Empty;
        private IRpcClient rpc => _rpcClientProvider.Client;

        public string SearchString
        {
            get => _searchString;
            set => this.RaiseAndSetIfChanged(ref _searchString, value);
        }


        private string _result;
        private IReadOnlyCollection<KeyValuePair<string, int>> _pairs;
        private double _progress;
        private bool _isProcessing;
        private string auth;
        private IRpcClientProvider _rpcClientProvider;

        public string Result
        {
            get => _result;
            set => this.RaiseAndSetIfChanged(ref _result, value);
        }

        public IReadOnlyCollection<KeyValuePair<string, int>> Pairs
        {
            get => _pairs;
            set => this.RaiseAndSetIfChanged(ref _pairs, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
        }


        private bool _loadingNFT = false;
        public bool LoadingNFT
        {
            get => _loadingNFT;
            set => this.RaiseAndSetIfChanged(ref _loadingNFT, value);
        }

        public byte[] NftData { get; set; }


        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }


        private Bitmap _bmp;
        public Bitmap Bmp
        {
            get => _bmp;
            set => this.RaiseAndSetIfChanged(ref _bmp, value);
        }

        private bool _canQueryHolders = false;
        public bool CanQueryHolders
        {
            get => _canQueryHolders;
            set => this.RaiseAndSetIfChanged(ref _canQueryHolders, value);
        }

        private List<AccountKeyPair> _mints;
        public List<AccountKeyPair> Mints
        {
            get => _mints;
            set
            {
                this.RaiseAndSetIfChanged(ref _mints, value);
                CanQueryHolders = _mints != null && _mints.Count > 0;
            }
        }

        public NFTViewModel(IRpcClientProvider rpcClientProvider)
        {
            _rpcClientProvider = rpcClientProvider;
        }

        public async void Save()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save NFT"
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var f = File.OpenWrite(selected))
                    {
                        await f.WriteAsync(NftData);
                    }
                }
            }
        }

        public async void SaveMetadata()
        {
            var sfd = new SaveFileDialog()
            {
                Title = "Save NFT Metadata",
                DefaultExtension = "json",
                Filters = new() { new FileDialogFilter() { Extensions = new() { "json" }, Name = "JSON Files" } },
                InitialFileName = SearchString,
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };


            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var selected = await sfd.ShowAsync(desktop.MainWindow);
                if (selected == null) return;
                if (selected.Length > 0)
                {
                    using (var s = new StreamWriter(selected))
                    {
                        await s.WriteAsync(Result);
                    }
                }
            }
        }

        public async void FindMints()
        {
            Mints = (await rpc.GetProgramAccountsAsync("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s",
                memCmpList: new List<MemCmp>() { new MemCmp() { Bytes = auth, Offset = 326 } }))?.Result;
        }

        public async void FindHolders()
        {


            Dictionary<string, int> accounts = new();
            int tot = 0;
            foreach (var mint in Mints)
            {

                var bytes = Convert.FromBase64String(mint.Account.Data[0]);

                var pkey = new ReadOnlySpan<byte>(bytes).GetPubKey(33);


                var largest = await rpc.GetTokenLargestAccountsAsync(pkey);

                var acc = await rpc.GetTokenAccountInfoAsync(largest.Result.Value[0].Address);

                var owner = acc.Result.Value.Data.Parsed.Info.Owner;

                int count = 0;
                accounts.TryGetValue(owner, out count);
                count++;
                accounts[owner] = count;
                tot++;
                Progress = (double)tot / Mints.Count * 100;
            }
            Pairs = accounts;
        }

        public async void QueryByAddress()
        {
            LoadingNFT = true;

            List<byte[]> seeds = new List<byte[]>();
            var metaProgram = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
            seeds.Add(System.Text.Encoding.UTF8.GetBytes("metadata"));
            seeds.Add(metaProgram.KeyBytes);
            seeds.Add(new PublicKey(SearchString).KeyBytes);
            AddressExtensions.TryFindProgramAddress(seeds, metaProgram.KeyBytes, out byte[] res, out _);

            var pk = new PublicKey(res);

            var metaAcc = await rpc.GetAccountInfoAsync(pk);
            var bytes = Convert.FromBase64String(metaAcc.Result.Value.Data[0]);

            var uri = ParseUri(bytes).Trim('\0');
            auth = GetAuthority(bytes);

            string json = "";
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(uri);

                json = Encoding.UTF8.GetString(data);
            }

            var jsonDoc = JsonDocument.Parse(json);
            var newJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });


            var img = jsonDoc.RootElement.GetProperty("image").GetString();

            using (var wc = new WebClient())
            {
                NftData = await wc.DownloadDataTaskAsync(img);

                Bmp = new Bitmap(new MemoryStream(NftData));

            }


            Result = newJson;

            LoadingNFT = false;
        }

        public async void Search()
        {
            IsProcessing = true;
            Progress = 0;
            List<byte[]> seeds = new List<byte[]>();
            var metaProgram = new PublicKey("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
            seeds.Add(System.Text.Encoding.UTF8.GetBytes("metadata"));
            seeds.Add(metaProgram.KeyBytes);
            seeds.Add(new PublicKey(SearchString).KeyBytes);
            AddressExtensions.TryFindProgramAddress(seeds, metaProgram.KeyBytes, out byte[] res, out _);

            var pk = new PublicKey(res);


            var tx = await rpc.GetTransactionAsync("45pGoC4Rr3fJ1TKrsiRkhHRbdUeX7633XAGVec6XzVdpRbzQgHhe6ZC6Uq164MPWtiqMg7wCkC6Wy3jy2BqsDEKf");

            var metaAcc = await rpc.GetAccountInfoAsync(pk);
            var bytes = Convert.FromBase64String(metaAcc.Result.Value.Data[0]);

            var uri = ParseUri(bytes).Trim('\0');
            auth = GetAuthority(bytes);

            string json = "";
            using (var wc = new WebClient())
            {
                var data = await wc.DownloadDataTaskAsync(uri);

                json = Encoding.UTF8.GetString(data);
            }

            var jsonDoc = JsonDocument.Parse(json);
            var newJson = JsonSerializer.Serialize(jsonDoc, new JsonSerializerOptions { WriteIndented = true });




            Result = newJson;


            IsProcessing = false;
            Progress = 0;

        }

        private string ParseUri(byte[] data)
        {

            var span = new ReadOnlySpan<byte>(data);
            var len = span.GetString(115, out var uri);
            var len2 = uri.Length;
            return uri;
        }

        private string GetAuthority(byte[] data)
        {
            var span = new ReadOnlySpan<byte>(data);
            return span.GetPubKey(326);
        }
    }
}
