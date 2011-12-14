CREATE TABLE sysuser (
	sysuser_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	sysuser_login NVARCHAR(64) NOT NULL,
	sysuser_password NVARCHAR(64) NULL,
	sysuser_firstname NVARCHAR(128) NULL,
	sysuser_surname NVARCHAR(128) NULL,
	sysuser_email NVARCHAR(128) NULL,
	sysuser_group_id UNIQUEIDENTIFIER NULL,
	sysuser_created DATETIME NOT NULL,
	sysuser_updated DATETIME NOT NULL,
	sysuser_created_by UNIQUEIDENTIFIER NULL,
	sysuser_updated_by UNIQUEIDENTIFIER NULL
);

CREATE TABLE sysgroup (
	sysgroup_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	sysgroup_parent_id UNIQUEIDENTIFIER NULL,
	sysgroup_name NVARCHAR(64) NOT NULL,
	sysgroup_description NVARCHAR(255) NULL,
	sysgroup_created DATETIME NOT NULL,
	sysgroup_updated DATETIME NOT NULL,
	sysgroup_created_by UNIQUEIDENTIFIER NULL,
	sysgroup_updated_by UNIQUEIDENTIFIER NULL,
	FOREIGN KEY (sysgroup_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (sysgroup_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE sysaccess (
	sysaccess_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	sysaccess_group_id UNIQUEIDENTIFIER NOT NULL,
	sysaccess_function NVARCHAR(64) NOT NULL,
	sysaccess_description NVARCHAR(255) NULL,
	sysaccess_created DATETIME NOT NULL,
	sysaccess_updated DATETIME NOT NULL,
	sysaccess_created_by UNIQUEIDENTIFIER NULL,
	sysaccess_updated_by UNIQUEIDENTIFIER NULL,
	FOREIGN KEY (sysaccess_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (sysaccess_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE sysparam (
	sysparam_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	sysparam_name NVARCHAR(64) NOT NULL,
	sysparam_value NVARCHAR(128) NULL,
	sysparam_description NVARCHAR(255) NULL,
	sysparam_created DATETIME NOT NULL,
	sysparam_updated DATETIME NOT NULL,
	sysparam_created_by UNIQUEIDENTIFIER NULL,
	sysparam_updated_by UNIQUEIDENTIFIER NULL,
	FOREIGN KEY (sysparam_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (sysparam_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE pagetemplate (
	pagetemplate_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	pagetemplate_name NVARCHAR(64) NOT NULL,
	pagetemplate_description NVARCHAR(255) NULL,
	pagetemplate_preview NTEXT NULL,
	pagetemplate_page_regions NVARCHAR(255) NULL,
	pagetemplate_global_regions NVARCHAR(255) NULL,
	pagetemplate_properties NVARCHAR(255) NULL,
	pagetemplate_view NVARCHAR(128) NULL,
	pagetemplate_view_show BIT NOT NULL default(0),
	pagetemplate_controller NVARCHAR(128) NULL,
	pagetemplate_controller_show BIT NOT NULL default(0),
	pagetemplate_redirect NVARCHAR(128) NULL,
	pagetemplate_redirect_show BIT NOT NULL default(0),
	pagetemplate_created DATETIME NOT NULL,
	pagetemplate_updated DATETIME NOT NULL,
	pagetemplate_created_by UNIQUEIDENTIFIER NOT NULL,
	pagetemplate_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (pagetemplate_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (pagetemplate_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE posttemplate (
	posttemplate_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	posttemplate_name NVARCHAR(64) NOT NULL,
	posttemplate_description NVARCHAR(255) NULL,
	posttemplate_preview NTEXT NULL,
	posttemplate_archive_name NVARCHAR(64) NULL,
	posttemplate_view NVARCHAR(128) NULL,
	posttemplate_controller NVARCHAR(128) NULL,
	posttemplate_manager_view NVARCHAR(128) NULL,
	posttemplate_manager_controller NVARCHAR(128) NULL,
	posttemplate_created DATETIME NOT NULL,
	posttemplate_updated DATETIME NOT NULL,
	posttemplate_created_by UNIQUEIDENTIFIER NOT NULL,
	posttemplate_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (posttemplate_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (posttemplate_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE permalink (
	permalink_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	permalink_parent_id UNIQUEIDENTIFIER NOT NULL,
	permalink_type NVARCHAR(16) NOT NULL,
	permalink_name NVARCHAR(128) NOT NULL,
	permalink_created DATETIME NOT NULL,
	permalink_updated DATETIME NOT NULL,
	permalink_created_by UNIQUEIDENTIFIER NOT NULL,
	permalink_updated_by UNIQUEIDENTIFIER NOT NULL
);
CREATE UNIQUE INDEX index_permalink_name ON permalink (permalink_name);

CREATE TABLE page (
	page_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	page_template_id UNIQUEIDENTIFIER NOT NULL,
	page_group_id UNIQUEIDENTIFIER NULL,
	page_parent_id UNIQUEIDENTIFIER NULL,
	page_seqno INT NOT NULL DEFAULT(1),
	page_title NVARCHAR(128) NOT NULL,
	page_navigation_title NVARCHAR(128) NULL,
	page_is_hidden BIT NOT NULL DEFAULT(0),
	page_keywords NVARCHAR(128) NULL,
	page_description NVARCHAR(255) NULL,
	page_controller NVARCHAR(128) NULL,
	page_view NVARCHAR(128) NULL,
	page_redirect NVARCHAR(128) NULL,
	page_created DATETIME NOT NULL,
	page_updated DATETIME NOT NULL,
	page_published DATETIME NULL,
	page_created_by UNIQUEIDENTIFIER NOT NULL,
	page_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (page_template_id) REFERENCES pagetemplate (pagetemplate_id),
	FOREIGN KEY (page_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (page_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE region (
	region_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	region_page_id UNIQUEIDENTIFIER NULL,
	region_name NVARCHAR(64) NOT NULL,
	region_body NTEXT NULL,
	region_created DATETIME NOT NULL,
	region_updated DATETIME NOT NULL,
	region_created_by UNIQUEIDENTIFIER NOT NULL,
	region_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (region_page_id) REFERENCES page (page_id),
	FOREIGN KEY (region_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (region_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE property (
	property_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	property_page_id UNIQUEIDENTIFIER NOT NULL,
	property_name NVARCHAR(64) NOT NULL,
	property_value NTEXT NULL,
	property_created DATETIME NOT NULL,
	property_updated DATETIME NOT NULL,
	property_created_by UNIQUEIDENTIFIER NOT NULL,
	property_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (property_page_id) REFERENCES page (page_id),
	FOREIGN KEY (property_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (property_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE post (
	post_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	post_template_id UNIQUEIDENTIFIER NOT NULL,
	post_title NVARCHAR(128) NOT NULL,
	post_excerpt NVARCHAR(255) NULL,
	post_body NTEXT NULL,
	post_created DATETIME NOT NULL,
	post_updated DATETIME NOT NULL,
	post_published DATETIME NOT NULL,
	post_created_by UNIQUEIDENTIFIER NOT NULL,
	post_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (post_template_id) REFERENCES posttemplate (posttemplate_id),
	FOREIGN KEY (post_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (post_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE content (
	content_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	content_filename NVARCHAR(128) NOT NULL,
	content_type NVARCHAR(64) NOT NULL,
	content_alt NVARCHAR(128) NULL,
	content_description NVARCHAR(255) NULL,
	content_created DATETIME NOT NULL,
	content_updated DATETIME NOT NULL,
	content_created_by UNIQUEIDENTIFIER NOT NULL,
	content_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (content_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (content_updated_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE upload (
	upload_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	upload_parent_id UNIQUEIDENTIFIER NULL,
	upload_filename NVARCHAR(128) NOT NULL,
	upload_type NVARCHAR(64) NOT NULL,
	upload_created DATETIME NOT NULL,
	upload_created_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (upload_created_by) REFERENCES sysuser (sysuser_id)
);

CREATE TABLE attachment (
	attachment_id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	attachment_content_id UNIQUEIDENTIFIER NOT NULL,
	attachment_parent_id UNIQUEIDENTIFIER NOT NULL,
	attachment_primary BIT NOT NULL DEFAULT(0),
	attachment_created DATETIME NOT NULL,
	attachment_updated DATETIME NOT NULL,
	attachment_created_by UNIQUEIDENTIFIER NOT NULL,
	attachment_updated_by UNIQUEIDENTIFIER NOT NULL,
	FOREIGN KEY (attachment_content_id) REFERENCES content (content_id),
	FOREIGN KEY (attachment_created_by) REFERENCES sysuser (sysuser_id),
	FOREIGN KEY (attachment_updated_by) REFERENCES sysuser (sysuser_id)
);

-- Default users
INSERT INTO sysuser (sysuser_id, sysuser_login, sysuser_created, sysuser_updated)
VALUES ('ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'sys', GETDATE(), GETDATE());
INSERT INTO sysuser (sysuser_id, sysuser_login, sysuser_firstname, sysuser_surname, sysuser_created, sysuser_updated)
VALUES ('4037dc45-90d2-4adc-84aa-593be867c29d', 'admin', 'byBrick', 'Administrator', GETDATE(), GETDATE());

-- Default templates
INSERT INTO pagetemplate (pagetemplate_id, pagetemplate_name, pagetemplate_description, pagetemplate_page_regions, 
	pagetemplate_created, pagetemplate_updated, pagetemplate_created_by, pagetemplate_updated_by)
VALUES ('906761ea-6c04-4f4b-9365-f2c350ff4372', 'Vanlig sida,Vanliga sidor', 'Standardsida med en HTML-yta.',
	'Innehåll', GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO posttemplate (posttemplate_id, posttemplate_name, posttemplate_description, posttemplate_created, 
	posttemplate_updated, posttemplate_created_by, posttemplate_updated_by)
VALUES ('03257c2a-3c91-4867-b20e-5fb9d3d97ab4', 'Vanligt inlägg,Vanliga inlägg', 'Standardinlägg.',
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
INSERT INTO posttemplate (posttemplate_id, posttemplate_name, posttemplate_description, posttemplate_archive_name,
	posttemplate_created, posttemplate_updated, posttemplate_created_by, posttemplate_updated_by)
VALUES ('5017dbe4-5685-4941-921b-ca922edc7a12', 'Nyhet,Nyheter', 'Nyhetsinlägg.', 'nyheter',
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');

-- Default page
INSERT INTO page (page_id, page_template_id, page_seqno, page_title, page_keywords, page_description,
	page_created, page_updated, page_published, page_created_by, page_updated_by)
VALUES ('7849b6d6-dc43-43f6-8b5a-5770ab89fbcf', '906761ea-6c04-4f4b-9365-f2c350ff4372', 1, 'Start', 
	'Piranha, byBrick', 'Välkommen till Piranha', GETDATE(), GETDATE(), GETDATE(),
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
-- Permalink
INSERT INTO permalink (permalink_id, permalink_parent_id, permalink_type, permalink_name, permalink_created,
	permalink_updated, permalink_created_by, permalink_updated_by)
VALUES ('1e64c1d4-e24f-4c7c-8f61-f3a75ad2e2fe', '7849b6d6-dc43-43f6-8b5a-5770ab89fbcf', 'PAGE', 'start',
	GETDATE(), GETDATE(), 'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');
-- Region
INSERT INTO region (region_id, region_page_id, region_name, region_body, region_created, region_updated,
	region_created_by, region_updated_by)
VALUES ('87ec4dbd-c3ba-4a6b-af49-78421528c363', '7849b6d6-dc43-43f6-8b5a-5770ab89fbcf', 'Innehåll',
	'<p>Välkommen till Piranha, den enkla och minimala content management lösningen för .NET.</p>', GETDATE(), GETDATE(),
	'ca19d4e7-92f0-42f6-926a-68413bbdafbc', 'ca19d4e7-92f0-42f6-926a-68413bbdafbc');