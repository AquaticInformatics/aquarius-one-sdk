using System;

namespace ONE.ClientSDK.Operations.Sample
{
    /// <summary>
    /// PropertyBag for an activity of activity types:
    ///     ActivityTypeSampleScheduled : 4c502a82-d8a3-4c64-a741-59e485b9bc0b
    ///     ActivityTypeSampleAdHoc     : 2102a6c9-9387-4107-8936-ef6c8340eeda
    /// </summary>
    public class ActivityCollectionPropertyBag
    {
        /// <summary>
        /// Used as flag to determine the activity type.
        /// If a value is provided, the type is ActivityTypeSampleScheduled
        /// If value is null, the type is ActivityTypeSampleAdHoc
        /// </summary>
        public Guid? ScheduleId { get; set; }

        // Properties that apply to all types
        public string Comment { get; set; }
        public NameIdPair Location { get; set; }
        public NameIdPair TestGroup { get; set; }

        // Properties that only apply to type ActivityTypeSampleScheduled
        public string DefaultCollectionTime { get; set; }
        public string CollectionTime { get; set; }

        // Properties that only apply to type ActivityTypeSampleAdHoc
        public string Name { get; set; }
        public int StatusCode { get; set; }
        public int Reason { get; set; }
        public int? SampleType { get; set; }

        // Properties that are used for tolerance
        public string OriginalScheduledDate { get; set; }
    }

    public class NameIdPair
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
