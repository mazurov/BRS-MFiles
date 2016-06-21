CREATE VIEW [dbo].mea_Authors
	AS SELECT CONVERT(nvarchar(36), Documents.DocumentId) as id, 'organization' as [type], Documents.Author as value  FROM Documents 
	WHERE NULLIF(Documents.Author , '') IS NOT NULL;
