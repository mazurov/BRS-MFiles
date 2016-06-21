ALTER VIEW [dbo].[mea_Documents]
	AS SELECT '1.0' as schemaVersion, CONVERT(nvarchar(36), Documents.DocumentId) as id,
	MFilesDocuments.CreatedDate as published, MFilesDocuments.ModifiedDate as updated,
	Documents.Convention as treaty, Documents.Copyright as copyright,'' as thumbnailUrl
	FROM [Documents] INNER JOIN MFilesDocuments ON MFilesDocuments.Guid = Documents.DocumentId
