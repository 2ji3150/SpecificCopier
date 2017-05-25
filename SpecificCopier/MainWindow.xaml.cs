using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Input;

namespace SpecificCopier {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.DataContext = vm;
        }
        public static RoutedCommand StartCommand = new RoutedCommand();
        public static RoutedCommand CancelCommand = new RoutedCommand();
        ViewModel vm = new ViewModel();
        private CancellationTokenSource cts;
        private void src_ofd_Click(object sender, RoutedEventArgs e) {
            using (var dialog = new CommonOpenFileDialog()) {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
                vm.Input = dialog.FileName;
            }
        }

        private void dst_ofd_Click(object sender, RoutedEventArgs e) {
            using (var dialog = new CommonOpenFileDialog()) {
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
                vm.Output = dialog.FileName;
            }
        }

        private async void StartCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            MessageBox.Show("start");
            vm.Idle = false;
            using (cts = new CancellationTokenSource()) {
                var token = cts.Token;
                var task = Task.Run(() => copyfiles(vm.Input, vm.Output, vm.CopyName, token));
                await task;
            }
            vm.Idle = true;
        }
        private void StartCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = Directory.Exists(vm.Input) && Directory.Exists(vm.Output) && !string.IsNullOrWhiteSpace(vm.CopyName);
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            cancel.IsEnabled = false;
            cts?.Cancel();
        }

        private void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = !vm.Idle;
        }

        private void copyfiles(string src, string dst, string searchPattern, CancellationToken token) {
            if (vm.ProgressValue != 0) vm.ProgressValue = 0;
            var copyfiles = Directory.GetFiles(src, searchPattern, SearchOption.AllDirectories);
            int total = copyfiles.Length;
            string dstcopyfile;
            uint count = 0;
            try {
                foreach (var file in copyfiles) {
                    token.ThrowIfCancellationRequested();
                    dstcopyfile = dst + file.Substring(src.Length);
                    Directory.CreateDirectory(Path.GetDirectoryName(dstcopyfile));
                    File.Copy(file, dstcopyfile, true);
                    vm.ProgressValue = (double)++count / total;
                }
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show("完成しました");
                vm.ProgressValue = 0;
            }
            catch (Exception ex) {
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show(ex.Message);
            }
        }
    }
}
