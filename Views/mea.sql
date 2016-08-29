/****** Object:  View [dbo].[mea_AuthorDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[mea_AuthorDocuments]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Documents.DocumentId) as authorId,  CONVERT(nvarchar(36), Documents.DocumentId) as documentId FROM dbo.Documents 
	WHERE NULLIF(Documents.Author , '') IS NOT NULL;

GO
/****** Object:  View [dbo].[mea_Authors]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Authors]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Documents.DocumentId) as id, 'organization' as [type], Documents.Author as value  FROM dbo.Documents 
	WHERE NULLIF(Documents.Author , '') IS NOT NULL;


GO
/****** Object:  View [dbo].[mea_Descriptions]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Descriptions]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Descriptions.DescriptionId) as id,CONVERT(nvarchar(36), Descriptions.Document_DocumentId) as documentId, Language as language, Value as value FROM dbo.Descriptions;

GO
/****** Object:  View [dbo].[mea_Documents]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Documents] WITH SCHEMABINDING
	AS SELECT '1.0' as schemaVersion, CONVERT(nvarchar(36), Documents.DocumentId) as id,
	MFilesDocuments.CreatedDate as published, MFilesDocuments.ModifiedDate as updated,
	Documents.Convention as treaty, Documents.Copyright as copyright,Files.ThumbnailUrl as thumbnailUrl, Documents.Country as country, 0 as displayOrder
	FROM dbo.[Documents] INNER JOIN dbo.MFilesDocuments ON MFilesDocuments.Guid = Documents.DocumentId INNER JOIN dbo.Files ON Documents.DocumentId = Files.FileId ;

GO
/****** Object:  View [dbo].[mea_Files]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Files]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Files.FileId) as id, CONVERT(nvarchar(36), Files.Document_DocumentId) as documentId, Files.Name as name, Files.Extension as extension,
	Files.MimeType as mimeType, Files.Size as size, Files.Url as url, Files.Language as language FROM  dbo.Files;


GO
/****** Object:  View [dbo].[mea_Identifiers]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Identifiers]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Documents.DocumentId) as id, CONVERT(nvarchar(36), Documents.DocumentId) as documentId, 'UN-Number' as identifierName, UnNumber as identifierValue FROM dbo.Documents;

GO
/****** Object:  View [dbo].[mea_KeywordDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_KeywordDocuments]
	WITH SCHEMABINDING
	AS SELECT CONVERT(NVARCHAR(36), PropertyId)  as keywordId, CONVERT(NVARCHAR(36), DocumentId) as documentId FROM dbo.DocumentsTerms;

GO
/****** Object:  View [dbo].[mea_Keywords]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Keywords]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, 'http://www.brsmeas.org/terms/' + Value  as termURI, 'brs' as scope, Value as termValueInEnglish FROM dbo.ListProperties INNER JOIN dbo.DocumentsTerms ON ListPropertyId=PropertyId;
GO
/****** Object:  View [dbo].[mea_ReferenceToEntity]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_ReferenceToEntity]
	AS SELECT CONVERT(NVARCHAR(36), ListProperties.ListPropertyId) as id, 'meeting' as entityType, ListProperties.Url as refURI FROM ListProperties INNER JOIN ListPropertyTypeListProperties ON ListProperties.ListPropertyId = ListPropertyTypeListProperties.ListProperty_ListPropertyId INNER JOIN ListPropertyTypes ON  ListPropertyTypeListProperties.ListPropertyType_ListPropertyTypeId = ListPropertyTypes.ListPropertyTypeId WHERE ListPropertyTypes.Name='meeting' AND  ListProperties.Url IS NOT NULL

GO
/****** Object:  View [dbo].[mea_ReferenceToEntityDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_ReferenceToEntityDocuments]
	WITH SCHEMABINDING
	AS SELECT CONVERT(NVARCHAR(36), PropertyId)  as referenceToEntityId, CONVERT(NVARCHAR(36), documentId) as documentId FROM dbo.DocumentsMeetings INNER JOIN dbo.ListProperties ON PropertyId=ListPropertyId WHERE ListProperties.Url IS NOT NULL

GO
/****** Object:  View [dbo].[mea_TagDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_TagDocuments]
	WITH SCHEMABINDING
	AS SELECT CONVERT(NVARCHAR(36), PropertyId)  as tagId, CONVERT(NVARCHAR(36), DocumentId) as documentId FROM dbo.DocumentsChemicals;


GO
/****** Object:  View [dbo].[mea_Tags]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Tags]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, ListProperties.Value as value, 'en' as language, 'chemical' as scope, '' as comment FROM  dbo.ListProperties INNER JOIN dbo.DocumentsChemicals ON ListProperties.ListPropertyId=DocumentsChemicals.PropertyId;

GO
/****** Object:  View [dbo].[mea_Titles]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Titles]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), Titles.TitleId) as id, CONVERT(nvarchar(36),Titles.Document_DocumentId) as documentId, Language as language, Value as value FROM dbo.Titles;

GO
/****** Object:  View [dbo].[mea_TypeDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_TypeDocuments]
	WITH SCHEMABINDING
	AS SELECT CONVERT(NVARCHAR(36), PropertyId)  as typeId, CONVERT(NVARCHAR(36), DocumentId) as documentId FROM dbo.DocumentsTypes;


GO
/****** Object:  View [dbo].[mea_Types]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Types] WITH SCHEMABINDING 
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, Value as value FROM dbo.ListProperties INNER JOIN dbo.DocumentsTypes ON DocumentsTypes.PropertyId=ListProperties.ListPropertyId;
