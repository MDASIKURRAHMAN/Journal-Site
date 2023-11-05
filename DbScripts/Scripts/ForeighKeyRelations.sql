use eJournalDb;


ALTER TABLE
    "Notifications" ADD CONSTRAINT "FK_Notifications_BlogId_Foreign" FOREIGN KEY("BlogId") REFERENCES "Blogs"("BlogId");
ALTER TABLE
    "Likes" ADD CONSTRAINT "FK_Likes_CommentId_Foreign" FOREIGN KEY("CommentId") REFERENCES "Comments"("CommentId");
ALTER TABLE
    "Users" ADD CONSTRAINT "FK_Users_ImageId_Foreign" FOREIGN KEY("ImageId") REFERENCES "Images"("ImageId");
ALTER TABLE
    "Comments" ADD CONSTRAINT "FK_Comments_blogId_Foreign" FOREIGN KEY("BlogId") REFERENCES "Blogs"("BlogId");
ALTER TABLE
    "Images" ADD CONSTRAINT "FK_Images_CommentId_Foreign" FOREIGN KEY("CommentId") REFERENCES "Comments"("CommentId");
ALTER TABLE
    "Images" ADD CONSTRAINT "FK_Images_BlogId_Foreign" FOREIGN KEY("BlogId") REFERENCES "Blogs"("BlogId");
ALTER TABLE
    "Likes" ADD CONSTRAINT "FK_Likes_UserId_Foreign" FOREIGN KEY("UserId") REFERENCES "Users"("UserId");
ALTER TABLE
    "Notifications" ADD CONSTRAINT "FK_Notifications_UserId_Foreign" FOREIGN KEY("UserId") REFERENCES "Users"("UserId");
ALTER TABLE
    "Blogs" ADD CONSTRAINT "FK_Blogs_Userid_Foreign" FOREIGN KEY("UserId") REFERENCES "Users"("UserId");
ALTER TABLE
    "Comments" ADD CONSTRAINT "FK_Comments_UserId_Foreign" FOREIGN KEY("UserId") REFERENCES "Users"("UserId");
ALTER TABLE
    "Comments" ADD CONSTRAINT "FK_Comments_ParentcommentId_Foreign" FOREIGN KEY("ParentCommentId") REFERENCES "Comments"("CommentId");
ALTER TABLE
    "Likes" ADD CONSTRAINT "FK_Likes_BlogId_Foreign" FOREIGN KEY("BlogId") REFERENCES "Blogs"("BlogId");