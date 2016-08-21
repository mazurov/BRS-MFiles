USE [BRS-Documents]
GO
/****** Object:  UserDefinedFunction [dbo].[fn_getCrmMeetingId]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Julien Hortoneda
-- Create date: 2015-08-21
-- Description:	returns the CRM meeting Id of a document, provided that vault and meeting accronym are provided
-- =============================================
CREATE FUNCTION [dbo].[fn_getCrmMeetingId] 
(
	@vault varchar(20) = '',
	@meetingAccronym varchar(50) = ''
)
RETURNS varchar(50)
AS
BEGIN
	-- Declare the return variable
	DECLARE @Result varchar(50);
	
	-- check params
	IF (@vault <> '' AND @meetingAccronym <> '')
		BEGIN
			-- set the xml with mappings
			DECLARE @myDoc xml;
			SET @myDoc = '<map>
					<treaty name="basel">
						<item>
							<meetingTitle>COP.12</meetingTitle>
							<meetingId>900C5B54-2046-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.11</meetingTitle>
							<meetingId>36F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.10</meetingTitle>
							<meetingId>3C89890A-8C27-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.9</meetingTitle>
							<meetingId>2EADBB80-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.8</meetingTitle>
							<meetingId>2BADBB80-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.7</meetingTitle>
							<meetingId>53B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.6</meetingTitle>
							<meetingId>56B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.5</meetingTitle>
							<meetingId>31ADBB80-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.4</meetingTitle>
							<meetingId>34ADBB80-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.3</meetingTitle>
							<meetingId>59B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.2</meetingTitle>
							<meetingId>5CB1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP.1</meetingTitle>
							<meetingId>37ADBB80-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.9</meetingTitle>
							<meetingId>02E0AFDF-2730-E311-B372-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.8</meetingTitle>
							<meetingId>47B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.7</meetingTitle>
							<meetingId>5FB1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.6</meetingTitle>
							<meetingId>62B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.5</meetingTitle>
							<meetingId>65B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.4</meetingTitle>
							<meetingId>68B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.3</meetingTitle>
							<meetingId>6BB1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.2</meetingTitle>
							<meetingId>6EB1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>OEWG.1</meetingTitle>
							<meetingId>71B1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CC.11</meetingTitle>
							<meetingId>DF978E33-DD42-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CC.10</meetingTitle>
							<meetingId>54106436-80E4-E211-829A-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CC.9</meetingTitle>
							<meetingId>4DB1B67A-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CC.8</meetingTitle>
							<meetingId>9589890A-8C27-E211-809D-0050569D5DE3</meetingId>
						</item>
				        
					</treaty>
				    
					<treaty name="rotterdam">
						<item>
							<meetingTitle>COP7</meetingTitle>
							<meetingId>900C5B54-2046-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP6</meetingTitle>
							<meetingId>36F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP5</meetingTitle>
							<meetingId>ACD5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP4</meetingTitle>
							<meetingId>CFD5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP3</meetingTitle>
							<meetingId>D2D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP2</meetingTitle>
							<meetingId>D5D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP1</meetingTitle>
							<meetingId>D8D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC11</meetingTitle>
							<meetingId>28F053CA-A806-E411-89D4-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC10</meetingTitle>
							<meetingId>A8107729-D842-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC9</meetingTitle>
							<meetingId>D97C4A94-CF9D-E211-975B-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC8</meetingTitle>
							<meetingId>49CBAAF7-8B27-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC7</meetingTitle>
							<meetingId>65BF93EE-2524-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC6</meetingTitle>
							<meetingId>E9BD81E1-7124-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC5</meetingTitle>
							<meetingId>D6516C76-7424-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC4</meetingTitle>
							<meetingId>E1D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC3</meetingTitle>
							<meetingId>E4D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC2</meetingTitle>
							<meetingId>E7D5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>CRC1</meetingTitle>
							<meetingId>EAD5F670-9E23-E211-809D-0050569D5DE3</meetingId>
						</item>
					</treaty>

					<treaty name="stockholm">
						<item>
							<meetingTitle>COP7</meetingTitle>
							<meetingId>900C5B54-2046-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP6</meetingTitle>
							<meetingId>36F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP5</meetingTitle>
							<meetingId>CAF6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP4</meetingTitle>
							<meetingId>71F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP3</meetingTitle>
							<meetingId>59F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP2</meetingTitle>
							<meetingId>83F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>COP1</meetingTitle>
							<meetingId>FAF6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC11</meetingTitle>
							<meetingId>AB741EB3-A806-E411-89D4-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC10</meetingTitle>
							<meetingId>DAFDA385-D842-E311-AC6E-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC9</meetingTitle>
							<meetingId>12C07BDA-8A27-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC8</meetingTitle>
							<meetingId>E8F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC7</meetingTitle>
							<meetingId>D4F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC6</meetingTitle>
							<meetingId>BBF6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC5</meetingTitle>
							<meetingId>A9F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC4</meetingTitle>
							<meetingId>6EF6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC3</meetingTitle>
							<meetingId>86F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC2</meetingTitle>
							<meetingId>89F6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
						<item>
							<meetingTitle>POPRC1</meetingTitle>
							<meetingId>8CF6BE74-9D23-E211-809D-0050569D5DE3</meetingId>
						</item>
					</treaty>
				    
				</map>';
			
			-- Get the crm meeting id from xml
			SET @Result =  @myDoc.value('(/map/treaty[@name = sql:variable("@vault")]/item[meetingTitle = sql:variable("@meetingAccronym")]/meetingId/text())[1]', 'varchar(50)' );
			
		END
	ELSE
		BEGIN
			SET @Result = null;
		END
		
	-- Return the result of the function
	RETURN @Result;

END

GO
/****** Object:  UserDefinedFunction [dbo].[fn_getDocDisplayOrder]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Julien Hortoneda
-- Create date: 2015-08-25
-- Description:	Define display order based on UN Number value (works only if UN Number has proper / and - to isolate meeting number in decisions)
-- =============================================
CREATE FUNCTION [dbo].[fn_getDocDisplayOrder] 
(
	@strDocNumber varchar(50) = ''
)
RETURNS int
AS
BEGIN
	
	DECLARE @count int
	DECLARE @NbPart int
	DECLARE @displayOrder int
	DECLARE @partNumber varchar(50)
	DECLARE @meetingNumber varchar(5)

	SET @count = 0
	SET @NbPart = 1
	SET @displayOrder = 0
	SET @partNumber = ''
	SET @meetingNumber = ''

	IF LEN(@strDocNumber) > 0
		BEGIN
			-- add a last / to terminate the digit extraction and extract 
			SET @strDocNumber = @strDocNumber + '/'
			-- Check if meeting number (string between - and following /) is in roman digits (only from I to X)
			IF CHARINDEX('-', @strDocNumber) > 0 AND CHARINDEX('/', @strDocNumber, CHARINDEX('-', @strDocNumber)) > 0
			BEGIN
				SET @meetingNumber = SUBSTRING(@strDocNumber, CHARINDEX('-', @strDocNumber) + 1, CHARINDEX('/', @strDocNumber, CHARINDEX('-', @strDocNumber)) - CHARINDEX('-', @strDocNumber) - 1)
				--PRINT 'Meeting Number is: ' + @meetingNumber
				IF PatIndex('%[^a-z]%', @meetingNumber) = 0
					BEGIN
						--PRINT 'meeting number is in roman numbers' 
						SET @strDocNumber = REPLACE(@strDocNumber, '-I/', '-1/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-II/', '-2/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-III/', '-3/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-IV/', '-4/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-V/', '-5/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-VI/', '-6/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-VII/', '-7/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-VIII/', '-8/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-IX/', '-9/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-X/', '-10/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XI/', '-11/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XII/', '-12/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XIII/', '-13/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XIV/', '-14/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XV/', '-15/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XVI/', '-16/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XVII/', '-17/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XVIII/', '-18/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XIX/', '-19/')
						SET @strDocNumber = REPLACE(@strDocNumber, '-XX/', '-20/')
						--PRINT 'meeting number is now: ' + @strDocNumber
					END
				END


			WHILE @count <= LEN(@strDocNumber)
			BEGIN 
				IF SUBSTRING(@strDocNumber, @count, 1)>='0' and SUBSTRING(@strDocNumber, @count, 1) <='9'
					BEGIN
						-- char is digit: store part number
						SET @partNumber = @partNumber + SUBSTRING(@strDocNumber, @count, 1)
					END
				ELSE
					BEGIN
						IF @partNumber <> 0
							BEGIN
								-- not a digit: build display order int and reset partNumber
								--PRINT 'partNb is: ' + @partNumber + ' - NbPart is: ' + CAST(@NbPart as varchar(50))
								--PRINT CAST(@partNumber as int) * (100000 / @NbPart)
								SET @displayOrder = @displayOrder + (CAST(@partNumber as int) * (10000 / (10 * @NbPart)))
								SET @NbPart = @NbPart * 10
								SET @partNumber = 0
							END
					END
				SET @count = @count + 1
			END
		END

	RETURN @displayOrder

END

GO
/****** Object:  View [dbo].[informea_BcDecisionsDocs]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_BcDecisionsDocs]
AS
SELECT     protocolVersion, id, link, type, status, number, treaty, published, updated, meetingId, meetingTitle, meetingUrl, displayOrder, BrsUnNumber, 
                      brsMeetingAccronym
FROM         dbo.informea_DecisionsDocs
WHERE     (treaty = N'basel')

GO
/****** Object:  View [dbo].[informea_DecisionsContent]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_DecisionsContent]
AS
SELECT        NULL AS id, DocumentId AS documentId, NULL AS language, NULL AS value
FROM            dbo.Documents
WHERE        (DocumentId IS NULL)

GO
/****** Object:  View [dbo].[informea_DecisionsDocs]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_DecisionsDocs]
AS
SELECT     1 AS protocolVersion, dbo.Documents.DocumentId AS id, NULL AS link, CASE CHARINDEX('/CRC.', Documents.UnNumber) 
                      WHEN 0 THEN 'decision' ELSE 'recommendation' END AS type, 'active' AS status, REVERSE(LEFT(REVERSE('/' + dbo.Documents.UnNumber), 
                      CHARINDEX('/', REVERSE('/' + dbo.Documents.UnNumber), CHARINDEX('-', REVERSE('/' + dbo.Documents.UnNumber))) - 1)) AS number, 
                      dbo.Documents.Convention AS treaty, dbo.Documents.PublicationDate AS published, dbo.Documents.PublicationDate AS updated,
                          (SELECT     dbo.fn_getCrmMeetingId(dbo.Documents.Convention, dbo.ListProperties.Value) AS Expr1) AS meetingId, NULL AS meetingTitle, NULL 
                      AS meetingUrl, dbo.Documents.UnNumber AS BrsUnNumber, dbo.ListProperties.Value AS brsMeetingAccronym,
                          (SELECT     dbo.fn_getDocDisplayOrder(dbo.Documents.UnNumber) AS Expr1) AS displayOrder
FROM         dbo.Documents INNER JOIN
                      dbo.TypeValueDocuments ON dbo.Documents.DocumentId = dbo.TypeValueDocuments.Document_DocumentId INNER JOIN
                      dbo.MeetingValueDocuments ON dbo.Documents.DocumentId = dbo.MeetingValueDocuments.Document_DocumentId INNER JOIN
                      dbo.ListProperties ON dbo.MeetingValueDocuments.MeetingValue_ListPropertyId = dbo.ListProperties.ListPropertyId
WHERE     (CHARINDEX('/', dbo.Documents.UnNumber) > 0) AND (CHARINDEX('/IG.', dbo.Documents.UnNumber) = 0) AND 
                      (dbo.TypeValueDocuments.TypeValue_ListPropertyId = '6B8DC1F3-2CBF-43F7-AC43-E510AC98BA73' OR
                      dbo.TypeValueDocuments.TypeValue_ListPropertyId = 'E926D163-0CED-4013-8BCE-0932CF1A5360') AND 
                      (dbo.ListProperties.Discriminator = N'MeetingValue') AND (CHARINDEX('Ex-', dbo.Documents.UnNumber) = 0)

GO
/****** Object:  View [dbo].[informea_DecisionsFiles]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_DecisionsFiles]
AS
SELECT     FileId AS id, Url AS url, MimeType AS mimeType, Language AS language, Name AS fileName, Document_DocumentId AS documentId
FROM         dbo.Files

GO
/****** Object:  View [dbo].[informea_DecisionsPrograms]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[informea_DecisionsPrograms]
AS
SELECT     dbo.ListProperties.ListPropertyId AS id, 
			(SELECT TOP(1) [brs_RelatedInformeaTerms] FROM [212.203.125.119].[AdxstudioPortals05015_MSCRM].[dbo].[brs_term] WHERE statecode = 0 and statuscode = 1 and  brs_name collate SQL_Latin1_General_CP1_CI_AS = ListProperties.Value collate SQL_Latin1_General_CP1_CI_AS) as informeaTermId, 
			'http://www.unep.org' AS namespace, dbo.ListProperties.Value AS term, 
                      dbo.ProgramValueDocuments.Document_DocumentId AS documentId
FROM         dbo.ProgramValueDocuments INNER JOIN
                      dbo.ListProperties ON dbo.ProgramValueDocuments.ProgramValue_ListPropertyId = dbo.ListProperties.ListPropertyId INNER JOIN
                      dbo.informea_DecisionsDocs ON dbo.ProgramValueDocuments.Document_DocumentId = dbo.informea_DecisionsDocs.id


GO
/****** Object:  View [dbo].[informea_DecisionsSummary]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_DecisionsSummary]
AS
SELECT     DescriptionId AS id, Document_DocumentId AS documentId, Language AS language, Value AS value
FROM         dbo.Descriptions

GO
/****** Object:  View [dbo].[informea_DecisionsTitles]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_DecisionsTitles]
AS
SELECT     TitleId AS id, Document_DocumentId AS documentId, Language AS language, Value AS value
FROM         dbo.Titles

GO
/****** Object:  View [dbo].[informea_RcDecisionsDocs]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_RcDecisionsDocs]
AS
SELECT     protocolVersion, id, link, type, status, number, treaty, published, updated, meetingId, meetingTitle, meetingUrl, displayOrder
FROM         dbo.informea_DecisionsDocs
WHERE     (treaty = N'rotterdam')

GO
/****** Object:  View [dbo].[informea_ScDecisionsDocs]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[informea_ScDecisionsDocs]
AS
SELECT     protocolVersion, id, link, type, status, number, treaty, published, updated, meetingId, meetingTitle, meetingUrl, displayOrder
FROM         dbo.informea_DecisionsDocs
WHERE     (treaty = N'stockholm')

GO
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
	Documents.Convention as treaty, Documents.Copyright as copyright,'' as thumbnailUrl, Documents.Country as country, 0 as displayOrder
	FROM dbo.[Documents] INNER JOIN dbo.MFilesDocuments ON MFilesDocuments.Guid = Documents.DocumentId;

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
	AS SELECT CONVERT(NVARCHAR(36), TermValue_ListPropertyId)  as keywordId, CONVERT(NVARCHAR(36), Document_DocumentId) as documentId FROM dbo.TermValueDocuments;

GO
/****** Object:  View [dbo].[mea_Keywords]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Keywords]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, 'http://www.brsmeas.org/terms/' + Value  as termURI, 'brs' as scope, Value as termValueInEnglish FROM dbo.ListProperties WHERE Discriminator='TermValue';
GO
/****** Object:  View [dbo].[mea_ReferenceToEntity]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_ReferenceToEntity]
	AS SELECT NULL as id, NULL as entityType, NULL as refURI WHERE 1=0;

GO
/****** Object:  View [dbo].[mea_ReferenceToEntityDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_ReferenceToEntityDocuments]
	WITH SCHEMABINDING
	AS SELECT NULL  as referenceToEntityId, CONVERT(NVARCHAR(36), Documents.DocumentId) as documentId FROM dbo.Documents WHERE 1=2;

GO
/****** Object:  View [dbo].[mea_TagDocuments]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_TagDocuments]
	WITH SCHEMABINDING
	AS SELECT CONVERT(NVARCHAR(36), ChemicalValue_ListPropertyId)  as tagId, CONVERT(NVARCHAR(36), Document_DocumentId) as documentId FROM dbo.ChemicalValueDocuments;


GO
/****** Object:  View [dbo].[mea_Tags]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Tags]
WITH SCHEMABINDING
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, ListProperties.Value as value, 'en' as language, 'chemical' as scope, '' as comment FROM  dbo.ListProperties WHERE Discriminator = 'ChemicalValue';

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
	AS SELECT CONVERT(NVARCHAR(36), TypeValue_ListPropertyId)  as typeId, CONVERT(NVARCHAR(36), Document_DocumentId) as documentId FROM dbo.TypeValueDocuments;


GO
/****** Object:  View [dbo].[mea_Types]    Script Date: 01/08/2016 08:16:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[mea_Types] WITH SCHEMABINDING 
	AS SELECT CONVERT(nvarchar(36), ListProperties.ListPropertyId) as id, Value as value FROM dbo.ListProperties WHERE Discriminator='TypeValue';

 
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "informea_DecisionsDocs"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 250
               Right = 218
            End
            DisplayFlags = 280
            TopColumn = 3
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 16
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2235
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_BcDecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_BcDecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Documents"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 176
               Right = 211
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsContent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsContent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[24] 4[28] 2[29] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Documents"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 195
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TypeValueDocuments"
            Begin Extent = 
               Top = 6
               Left = 233
               Bottom = 84
               Right = 437
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "MeetingValueDocuments"
            Begin Extent = 
               Top = 6
               Left = 475
               Bottom = 84
               Right = 693
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ListProperties"
            Begin Extent = 
               Top = 84
               Left = 233
               Bottom = 177
               Right = 384
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 16
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Files"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsFiles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsFiles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "ProgramValueDocuments"
            Begin Extent = 
               Top = 74
               Left = 308
               Bottom = 152
               Right = 528
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ListProperties"
            Begin Extent = 
               Top = 25
               Left = 577
               Bottom = 118
               Right = 728
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "informea_DecisionsDocs"
            Begin Extent = 
               Top = 43
               Left = 50
               Bottom = 240
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsPrograms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsPrograms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Descriptions"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Titles"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 114
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsTitles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_DecisionsTitles'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "informea_DecisionsDocs"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 277
               Right = 218
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_RcDecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_RcDecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "informea_DecisionsDocs"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 276
               Right = 218
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_ScDecisionsDocs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'informea_ScDecisionsDocs'
GO
