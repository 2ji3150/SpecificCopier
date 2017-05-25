namespace SpecificCopier {
    class ViewModel : BindableBase {
        private double _progressvalue;
        private string _intput;
        private string _output;
        private bool _idle = true;

        public string CopyName { get; set; }

        public string Input {
            get => _intput; set => SetField(ref _intput, value);
        }

        public string Output {
            get => _output; set => SetField(ref _output, value);
        }

        public double ProgressValue {
            get => _progressvalue; set => SetField(ref _progressvalue, value);
        }

        public bool Idle {
            get => _idle; set => SetField(ref _idle, value);
        }
    }
}
