using DynamicData;
using OneWare.Vcd.Parser.Data;

namespace OneWare.Vcd.Viewer.Models
{
    public class ExtendedVcdScopeModel : VcdScopeModel
    {
        public ExtendedVcdScopeModel(VcdScope scope)
            : base(scope)
        {
            // Initialize extended signals with BitIndices
            ExtendedSignals = scope.Signals.Select(signal => new ExtendedSignal(signal)).ToList();
        }

        // Property for the extended signals
        public List<ExtendedSignal> ExtendedSignals { get; }

        public class ExtendedSignal
        {
            public ExtendedSignal(IVcdSignal signal)
            {
                OriginalSignal = signal;
            }

            public IVcdSignal OriginalSignal { get; }

            public List<int> BitIndices { get; set; }
            public int BitIndexId { get; set; }
        }
    }
}