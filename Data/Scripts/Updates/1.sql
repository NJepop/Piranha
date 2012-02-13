-- Post
ALTER TABLE post ADD post_draft BIT NOT NULL default(1);
ALTER TABLE post ADD post_last_published DATETIME NULL;
ALTER TABLE post DROP CONSTRAINT pk_post_id;
ALTER TABLE post ADD CONSTRAINT pk_post_id PRIMARY KEY (post_id, post_draft);

-- Attachment
ALTER TABLE attachment ADD attachment_draft BIT NOT NULL default(1);
ALTER TABLE attachment DROP CONSTRAINT pk_attachment_id;
ALTER TABLE attachment ADD CONSTRAINT pk_attachment_id PRIMARY KEY (attachment_id, attachment_draft);
