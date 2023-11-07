using System;

namespace P5.SharePoint.Core.Models
{
    public class SpListItem
    {
        public int? Id { get; set; }
        public string DisplayName { get; set; }
        public string ContentType { get; set; }
        public string ContentTypeId { get; set; }
        public string FileSystemObjectType { get; set; }
        public string Title { get; set; }
        public string RelativeUrl { get; set; }
        public string FolderRelativeUrl { get; set; }
        public string RootFolderRelativeUrl { get; set; }
        public string LinkPath { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? LastNotificationDate { get; set; }
        public string PortalName { get; set; }
        public string RevisionNumber { get; set; }
        public string DocumentSetId { get; set; }
        public string DocumentSetTitle { get; set; }
        public string DocumentTypeTitle { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string AircraftFamily { get; set; }
        public string ModelNumber { get; set; }
        public string SubModel { get; set; }
        public string MainAtaSns { get; set; }
        public string SubAtaSns { get; set; }
        public string Effectivity { get; set; }
        public string Etops { get; set; }
        public string Criticality { get; set; }
        public string PartNumber { get; set; }
        public string PartSerialNumber { get; set; }
        public string WorkingGroup { get; set; }
        public string Supplier { get; set; }
        public string CorrectiveActionRequired { get; set; }
        public bool? AvailableOnPortal { get; set; }
        public bool? NotificationSent { get; set; }
        public int? FolderChildCount { get; set; }
        public int? ItemChildCount { get; set; }
        public Guid? FileId { get; set; }
        public string FileName { get; set; }
        public string FileContentType { get; set; }
        public long? FileLength { get; set; }
        public bool? FileInFolder { get; set; }

        public override string ToString()
        {
            return $"{DisplayName}: {FileSystemObjectType}";
        }
    }
}
