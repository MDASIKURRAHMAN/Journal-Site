$(document).ready(function () {
    let commentClickEventBlogId = null;
    let commentCountSpan = null;
    let commentEditDetails = {}; // it will hold the commentTextElement and CommentTextAreaElement.
    let activeEditCommentId = null;
    let activeReplyCommentId = null;

    TopPost();
    TopPopularPost();
    getNotification();

    let allPosts = $(".card-text");
    for (let post of allPosts) {
        let temp = post.innerHTML.replaceAll("&lt;", "<");
        temp = temp.replaceAll("&gt;", ">");
        post.innerHTML = temp;
    }

    $(".see-more").click(function (event) {
        let capped_div = $(event.target).parent().parent();
        let upcapped_div = capped_div.next();
        capped_div.hide();
        upcapped_div.show();
    })

    $(".see-less").click(function (event) {
        let uncapped_div = $(event.target).parent().parent();
        let capped_div = uncapped_div.prev();
        uncapped_div.hide();
        capped_div.show();
    })

    $("#filter-my-creations").click(function () {
        const checkbox = document.querySelector('#filter-my-creations');
        if (checkbox.checked) {
            let url = "/?filterMyCreations=true";
            window.location.replace(url);
        } else {
            let url = "/";
            window.location.replace(url);
        }
    })

    $(".comment-toggle").click(function (event) {
        debugger;
        let commentUtilitiesHolder = $(event.currentTarget).parent().parent().next();
        $(commentUtilitiesHolder).toggle();
    })

    $("#create-blog-input-id, #create-blog-button-id").click(function () {
        $.ajax({
            url: "/Home/Create",
            type: "GET",
            success: function (data) {
                $("#create-modal-holder").append(`${data}`);
                $("#create-blog-modal").modal("show");
                $(".close").click(function () {
                    $("#create-blog-modal").modal("hide");
                    $("#create-modal-holder").empty();
                })
                $(".create-modal-blog-image").on('change', imageHolderMaker);

                let tooltipTriggerList = [].slice.call(
                    document.querySelectorAll('[data-bs-toggle="tooltip"]')
                );
                let tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
                    return new bootstrap.Tooltip(tooltipTriggerEl);
                });
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    })

    function imageHolderMaker() {
        let imageElement = document.getElementsByClassName("create-modal-blog-image")[0];
        let imageHolder = "";
        for (let file of imageElement.files) {
            let oneImage = `[#image_${file.name}]`;
            imageHolder += oneImage;
        }
        //remove the previous ones first and recalculate the caretPosition.
        const textbox = $("#create-modal-blog-text");
        let caretPos = textbox[0].selectionStart;
        let textboxText = textbox.val();
        let allMatches = textboxText.match(/\[#image_.*\]/ig);
        let foundAt = textboxText.search(/\[#image_.*\]/ig);
        if (allMatches != null && foundAt < caretPos) {
            let totalLength = 0;
            for (let match of allMatches) {
                totalLength += match.length;
            }
            caretPos -= totalLength;
        }
        textboxText = textboxText.replaceAll(/\[#image_.*\]/ig, "");
        //add the latest ones.
        textbox.val(textboxText.substring(0, caretPos) + imageHolder + textboxText.substring(caretPos));
    }

    $(".create-or-show-comment").click(function (event) {
        debugger;
        $(event.currentTarget).hide();
        $(event.currentTarget).next().show();
        commentCountSpan = $(event.currentTarget).prev();

        $.ajax({
            url: "/Comment/Create",
            type: "get",
            success: function (data) {
                debugger;
                $(event.currentTarget).parent().parent().next().append(`${data}`);
                commentClickEventBlogId = $(event.currentTarget).prev().prev().val();
                $(".comment-create-button").click(commentCreateHandler);
                //get all the comments from backend and show them.
                let commentUtilityHolder = $(event.currentTarget).parent().parent().next();
                fetchAllComments(commentUtilityHolder, commentClickEventBlogId);
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText);
            }
        })
    })

    function commentCreateHandler(event) {
        debugger;
        let commentText = $(event.currentTarget).prev().val();
        let blogId = commentClickEventBlogId;
        let commentBoxForm = $(event.currentTarget).parent().parent().parent();
        $.ajax({
            url: "/Comment/Create",
            type: "post",
            data: { commentText, blogId },
            success: function (data) {
                //clear the comment box
                $(event.currentTarget).prev().val("");
                //append the data after the comment box
                let date = new Date(data.createdAt);
                let options = {
                    weekday: "long", year: "numeric", month: "short",
                    day: "numeric", hour: "2-digit", minute: "2-digit"
                };
                data.createdAt = date.toLocaleTimeString("en-us", options);
                debugger;
                let commentElement = createDisplayComment(data);
                commentBoxForm.parent().append(commentElement);

                incrementCommentCount();
                rebuildCommentClickEvents();
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    }

    function fetchAllComments(commentUtilityHolder, blogId) {
        $.ajax({
            url: "/Comment/GetAllCommentsByBlogId/?blogId=" + blogId,
            type: "get",
            success: function (data) {
                for (let comment of data) {
                    //fetched comment showing function goes here
                    let date = new Date(comment.createdAt);
                    let options = {
                        weekday: "long", year: "numeric", month: "short",
                        day: "numeric", hour: "2-digit", minute: "2-digit"
                    };
                    comment.createdAt = date.toLocaleTimeString("en-us", options);

                    $.ajax({
                        async: false,
                        url: "/Comment/GetAllCommentsByCommentId?commentId=" + comment.commentId,
                        type: "get",
                        success: function (repliesOfAComment) {
                            let replies = "";
                            for (let reply of repliesOfAComment) {
                                let replyDate = new Date(reply.createdAt);
                                let options = {
                                    weekday: "long", year: "numeric", month: "short",
                                    day: "numeric", hour: "2-digit", minute: "2-digit"
                                };
                                reply.createdAt = replyDate.toLocaleTimeString("en-us", options);
                                replies += makeReplyDivElement(reply);
                            }
                            replies = `<div class=reply-holder>${replies}</div>`
                            let commentElement = createDisplayComment(comment, replies);
                            commentUtilityHolder.append(commentElement);

                            rebuildCommentClickEvents();

                        },
                        error: function (xhs, status, error) {
                            alert(xhs.responseText);
                        }
                    })
                    
                }

                rebuildCommentClickEvents();

            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    }

    function createDisplayComment(comment, replies) {
        let editAble = "";
        let deleteAble = "";
        let likeInComment = "";
        let replyInComment = `<span class="comment-reply-button btn btn-secondary mx-2" data-commentId="${comment.commentId}">Reply</span>`;
        
        if (comment.loggedInUserId == comment.userId) {
            editAble = `<span class="comment-edit-button btn btn-secondary mx-2" data-commentId="${comment.commentId}">Edit</span>`;
            deleteAble = `<span class="comment-delete-button btn btn-secondary mx-2" data-commentId="${comment.commentId}">Delete</span>`;
        }

        if (typeof replies === "undefined") {
            replies = "<div class='reply-holder'></div>";
        }

        if (comment.isLikedByMe) {
            // is liked by me, 
            likeInComment = `
                <input class="commentId" value=${comment.commentId} hidden />
                <span class="comment-like-count">${comment.totalLikes}</span>
                <span class="comment-like-delete mx-1"> Likes<i class="mx-2 fa-solid fa-thumbs-up"></i></span >
                <span class="comment-like-create my-custom-hide mx-1"> Likes<i class="mx-2 fa-regular fa-thumbs-up"></i></span>
                `
        } else {
            // is not liked by me
            likeInComment = `
                <input class="commentId" value=${comment.commentId} hidden />
                <span class="comment-like-count">${comment.totalLikes}</span>
                <span class="comment-like-delete my-custom-hide  mx-1"> Likes<i class="mx-2 fa-solid fa-thumbs-up"></i></span >
                <span class="comment-like-create  mx-1"> Likes<i class="mx-2 fa-regular fa-thumbs-up"></i></span>
                `
        }

        let commentElement = `
        <div class="comment-and-reply">
            <div class="comment-display my-2">
                <span class="comment-user-image"><img src="${comment.userImage}" height="30px" width="30px" /></span>
                <div class="comment-username-text">
                    <div class="comment-username-createdat">
                        <h5 class="comment-display-username">${comment.userName}</h5>
                        <p class="comment-display-createdat">${comment.createdAt}</p>
                        ${editAble}
                        ${deleteAble}
                        ${likeInComment}
                        ${replyInComment}
                    </div>
                    <div>
                        <p class="comment-display-commenttext">${comment.commentText}</p>
                        <div class="comment-textarea-edit-button my-custom-hide my-1" >
                            <textarea class="comment-text-area comment-edit-area" asp-for="CommentText" rows="3"></textarea>
                            <a class="btn btn-dark comment-edit-submit-button" data-comment-id="${comment.commentId}">Update</a>
                        </div>
                    </div>
                </div>
            </div>
            <div class="comment-reply-textarea my-custom-hide my-1" >
                <textarea class="reply-text-area" rows="3"></textarea>
                <a class="btn btn-dark comment-reply-submit-button" data-comment-id="${comment.commentId}">Reply</a>
            </div>
            ${replies}        
        </div>
        `
        return commentElement;
    }


    // comment edit functionality goes here

    function commentEditHandler(event) {
        //alert("clicked on right button")
        let commentId = $(event.currentTarget).attr("data-commentId");

        if (commentId != activeEditCommentId) {
            $(commentEditDetails.commentTextAreaElement).removeClass("my-custom-show");
            $(commentEditDetails.commentTextAreaElement).addClass("my-custom-hide");
            $(commentEditDetails.commentTextElement).show();
        }
        let commentTextElement = $(event.currentTarget).parent().next().children().eq(0);
        let commentTextAreaElement = $(event.currentTarget).parent().next().children().eq(1);
        $(commentTextElement).toggle();
        $(commentTextAreaElement).toggleClass(["my-custom-hide", "my-custom-show"]);
        $(commentTextAreaElement).children().eq(0).val($(commentTextElement)[0].innerText);
        commentEditDetails.commentTextElement = commentTextElement;
        commentEditDetails.commentTextAreaElement = commentTextAreaElement;
        $(commentTextAreaElement).find(".comment-text-area").focus();
        activeEditCommentId = commentId;
    }

    function commentEditSubmitHandler(event) {
        //make the ajax call. if successfull then toggle the elements with the changed text.
        let modifiedText = $(event.currentTarget).prev().val();
        let commentId = $(event.currentTarget).attr("data-comment-id");
        let commentObject = {
            commentId: commentId,
            modifiedText: modifiedText
        };
        $.ajax({
            url: "/Comment/Edit/" + commentId,
            type: "post",

            data: commentObject,
            success: function (data) {
                $(commentEditDetails.commentTextElement)[0].innerText = modifiedText;
                $(commentEditDetails.commentTextElement).show();
                $(commentEditDetails.commentTextAreaElement).hide();
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText);
            }
        })
    }

    function commentDeleteHandler(event) {
        var result = confirm("Want to delete?");
        if (result) {
            // grab the values
            let commentId = $(event.currentTarget).attr("data-commentId");
            // make the ajax call
            $.ajax({
                url: "/Comment/Delete/",
                type: "post",
                data: { commentId },
                success: function (data) {
                    //delete the comment display div
                    let parentElement = $(event.currentTarget).parent().parent().parent();
                    $(parentElement).siblings(".reply-holder").remove();
                    $(parentElement).remove();
                    decrementCommentCount();
                },
                error: function (xhs, status, error) {
                    alert(xhs.responseText);
                }
            })
        }
    }

    function rebuildCommentClickEvents() {
        // remove the previous binds
        $(".comment-edit-button").off("click");
        $(".comment-edit-submit-button").off("click");
        $(".comment-delete-button").off("click");
        $(".comment-like-create").off("click");
        $(".comment-like-delete").off("click");
        $(".comment-reply-button").off("click");
        $(".comment-reply-submit-button").off("click");

        // attach the bind again
        $(".comment-edit-button").click(commentEditHandler);
        $(".comment-edit-submit-button").click(commentEditSubmitHandler);
        $(".comment-delete-button").click(commentDeleteHandler);
        $(".comment-like-create").click(createLikeInComment);
        $(".comment-like-delete").click(deleteLikeInComment);
        $(".comment-reply-button").click(replyButtonClickHandler);  
        $(".comment-reply-submit-button").click(replySubmitButtonHandler);  
    }

    function incrementCommentCount() {
        let totalComment = parseInt($(commentCountSpan)[0].innerText);
        totalComment += 1;
        $(commentCountSpan)[0].innerText = totalComment;
    }

    function decrementCommentCount() {
        let totalComment = parseInt($(commentCountSpan)[0].innerText);
        totalComment -= 1;
        $(commentCountSpan)[0].innerText = totalComment;
    }

    // like related functionalities goes here
    $(".liked").click(likeDeleteHandler);
    $(".unliked").click(likeCreateHandler);

    function likeCreateHandler(event) {
        let blogId = $(event.currentTarget).parent().find(".blog-id").val();
        let likeCountSpanElement = $(event.currentTarget).parent().find(".like-count");
        let unlikedSpanElement = $(event.currentTarget);
        let likedSpanElement = $(event.currentTarget).siblings(".liked")

        $.ajax({
            url: "/Like/Create",
            type: "post",
            data: { blogId },
            success: function (data) {
                incrementLikeCount(likeCountSpanElement);
                // toggle the liked and unliked icon spans
                $(unlikedSpanElement).hide();
                $(likedSpanElement).show();
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText);
            }
        })
    }

    function likeDeleteHandler(event) {
        let blogId = $(event.currentTarget).parent().find(".blog-id").val();
        let likeCountSpanElement = $(event.currentTarget).parent().find(".like-count");
        let likedSpanElement = $(event.currentTarget);
        let unlikedSpanElement = $(event.currentTarget).siblings(".unliked")

        $.ajax({
            url: "/Like/Delete/",
            type: "post",
            data: { blogId },
            success: function (data) {
                decrementLikeCount(likeCountSpanElement);
                // toggle the liked and unliked icon spans
                $(unlikedSpanElement).show();
                $(likedSpanElement).hide();
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText);
            }
        })
    }

    function incrementLikeCount(likeCountSpanElement) {
        let totalLike = parseInt($(likeCountSpanElement)[0].innerText);
        totalLike += 1;
        $(likeCountSpanElement)[0].innerText = totalLike;
    }

    function decrementLikeCount(likeCountSpanElement) {
        let totalLike = parseInt($(likeCountSpanElement)[0].innerText);
        totalLike -= 1;
        $(likeCountSpanElement)[0].innerText = totalLike;
    }

    function incrementLikeCount(likeCountSpanElement) {
        let totalLike = parseInt($(likeCountSpanElement)[0].innerText);
        totalLike += 1;
        $(likeCountSpanElement)[0].innerText = totalLike;
    }

    function decrementLikeCount(likeCountSpanElement) {
        let totalLike = parseInt($(likeCountSpanElement)[0].innerText);
        totalLike -= 1;
        $(likeCountSpanElement)[0].innerText = totalLike;
    }

    //Like in comment related functionalities goes here.
    function createLikeInComment(event) {
        let commentId = $(event.currentTarget).siblings(".commentId").val();
        $.ajax({
            url: "Like/Create",
            type: "post",
            data: { commentId },
            success: function (data) {
                $(event.currentTarget).hide();
                $(event.currentTarget).siblings(".comment-like-delete").show();
                incrementCommentLikeCount(event.currentTarget);
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    }

    function deleteLikeInComment(event) {
        let commentId = $(event.currentTarget).siblings(".commentId").val();
        $.ajax({
            url: "Like/Delete",
            type: "post",
            data: { commentId },
            success: function (data) {
                $(event.currentTarget).hide();
                $(event.currentTarget).siblings(".comment-like-create").show();
                decrementCommentLikeCount(event.currentTarget);
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    }

    function incrementCommentLikeCount(eventElement) {
        let commentLikeCount = parseInt($(eventElement).siblings(".comment-like-count")[0].innerText);
        commentLikeCount += 1;
        $(eventElement).siblings(".comment-like-count")[0].innerText = commentLikeCount;
    }

    function decrementCommentLikeCount(eventElement) {
        let commentLikeCount = parseInt($(eventElement).siblings(".comment-like-count")[0].innerText);
        commentLikeCount -= 1;
        $(eventElement).siblings(".comment-like-count")[0].innerText = commentLikeCount;
    }

    // reply on comment related functions goes here
    function replyButtonClickHandler(event) {
        let commentId = $(event.currentTarget).attr("data-commentid")
        let parentComment = $(event.currentTarget).parents(".comment-display");
        if (commentId != activeReplyCommentId) {
            $(".comment-reply-textarea").removeClass("my-custom-show");
            $(".comment-reply-textarea").addClass("my-custom-hide");
            $(parentComment).next().addClass("my-custom-show");
            activeReplyCommentId = commentId;
            let replyTextAreaElement = $(event.currentTarget).parents(".comment-display").siblings(".comment-reply-textarea").children(".reply-text-area");
            let displayValue = $(replyTextAreaElement).parent().css('display');
            if (displayValue == "flex") {
                $(replyTextAreaElement).focus();
            }
        }
        else {
            $(".comment-reply-textarea").removeClass("my-custom-show");
            $(".comment-reply-textarea").addClass("my-custom-hide");
            activeReplyCommentId = null;
        }

        
    }

    function replySubmitButtonHandler(event) {
        let repliedText = $(event.currentTarget).prev().val();
        let commentId = $(event.currentTarget).attr("data-comment-id");

        $.ajax({
            url: "/Comment/Create",
            type: "post",
            data: {
                "commentText": repliedText,
                "commentId": commentId
            },
            success: function (data) {
                //todo
                let date = new Date(data.createdAt);
                let options = {
                    weekday: "long", year: "numeric", month: "short",
                    day: "numeric", hour: "2-digit", minute: "2-digit"
                };
                data.createdAt = date.toLocaleTimeString("en-us", options);
                let parendElement = $(event.currentTarget).parents(".comment-reply-textarea");
                let replyHolderElement = $(parendElement).siblings(".reply-holder");
                $(event.currentTarget).siblings(".reply-text-area").val("");
                $(event.currentTarget).parent().hide();
                $(replyHolderElement).append(makeReplyDivElement(data));

                rebuildCommentClickEvents();
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText)
            }
        })
    }

    function getRepliesOfComment(commentId) {
        $.ajax({
            url: "/Comment/GetAllCommentsByCommentId?commentId=" + commentId,
            type: "get",
            success: function (data) {
                let replies = "";
                for (let reply of data) {
                    let replyDivElement = makeReplyDivElement(reply);
                    replies += replyDivElement;
                }
                return replies;
            },
            error: function (xhs, status, error) {
                alert(xhs.responseText);
            }
        })
    }

    function makeReplyDivElement(reply) {
        let editAble = "";
        let deleteAble = "";
        let likeInComment = "";

        if (reply.loggedInUserId == reply.userId) {
            editAble = `<span class="comment-edit-button btn btn-secondary mx-2" data-commentId="${reply.commentId}">Edit</span>`;
            deleteAble = `<span class="comment-delete-button btn btn-secondary mx-2" data-commentId="${reply.commentId}">Delete</span>`;
        }

        if (reply.isLikedByMe) {
            // is liked by me, 
            likeInComment = `
                <input class="commentId" value=${reply.commentId} hidden />
                <span class="comment-like-count">${reply.totalLikes}</span>
                <span class="comment-like-delete mx-1"> Likes<i class="mx-2 fa-solid fa-thumbs-up"></i></span >
                <span class="comment-like-create my-custom-hide mx-1"> Likes<i class="mx-2 fa-regular fa-thumbs-up"></i></span>
                `
        } else {
            // is not liked by me
            likeInComment = `
                <input class="commentId" value=${reply.commentId} hidden />
                <span class="comment-like-count">${reply.totalLikes}</span>
                <span class="comment-like-delete my-custom-hide  mx-1"> Likes<i class="mx-2 fa-solid fa-thumbs-up"></i></span >
                <span class="comment-like-create  mx-1"> Likes<i class="mx-2 fa-regular fa-thumbs-up"></i></span>
                `
        }

        let replyElement = `
        <div class="comment-display my-2">
            <span class="comment-user-image"><img src="${reply.userImage}" height="30px" width="30px" /></span>
            <div class="comment-username-text">
                <div class="comment-username-createdat">
                    <h5 class="comment-display-username">${reply.userName}</h5>
                    <p class="comment-display-createdat">${reply.createdAt}</p>
                    ${editAble}
                    ${deleteAble}
                    ${likeInComment}
                </div>
                <div>
                    <p class="comment-display-commenttext">${reply.commentText}</p>
                    <div class="comment-textarea-edit-button reply-textarea-edit-button my-custom-hide my-1" >
                        <textarea class="comment-text-area reply-edit-area" asp-for="CommentText" rows="3"></textarea>
                        <a class="btn btn-dark comment-edit-submit-button" data-comment-id="${reply.commentId}">Update</a>
                    </div>
                </div>
            </div>
        </div>
        `
        return replyElement;
    }

})

function updateBlog(blogId) {

    $.ajax({
        url: "/Home/Update?blogId=" + blogId,
        type: "GET",
        success: function (result) {
            $("#update-modal-holder").append(`${result}`);
            $("#update-blog-modal").modal("show");
            $(".close").click(function () {
                $("#update-blog-modal").modal("hide");
                $("#update-modal-holder").empty();
            })
            //  imageHolderMakerupdate();
            $(".update-modal-blog-image").on('change', imageHolderMakerupdate);

            let tooltipTriggerList = [].slice.call(
                document.querySelectorAll('[data-bs-toggle="tooltip"]')
            );
            let tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function imageHolderMakerupdate() {
    let imageElement = document.getElementsByClassName("update-modal-blog-image")[0];
    let imageHolder = "";
    for (let file of imageElement.files) {
        let oneImage = `[#image_${file.name}]`;
        imageHolder += oneImage;
    }
    //remove the previous ones first and recalculate the caretPosition.

    const textbox = $("#update-modal-blog-text");
    let caretPos = textbox[0].selectionStart;
    let textboxText = textbox.val();
    let allMatches = textboxText.match(/(<img[^>]+>)|(\[#image_.*\])/gi);
    let foundAt = textboxText.search(/(<img[^>]+>)|(\[#image_.*\])/gi);
    if (allMatches != null && foundAt < caretPos) {
        let totalLength = 0;
        for (let match of allMatches) {
            totalLength += match.length;
        }
        caretPos -= totalLength;
    }
    textboxText = textboxText.replaceAll(/(<img[^>]+>)|(\[#image_.*\])/gi, "");
    textbox.val(textboxText.substring(0, caretPos) + imageHolder + textboxText.substring(caretPos));

}
function DeleteBlog(blogId) {
    if (confirm("Are you sure you want to delete this post?")) {

        var url = "/Home/Delete?blogId=" + blogId;
        $.ajax({
            type: "POST",
            url: url,
            success: function (result) {
                location.reload();
            },
            error: function (xhr, status, error) {
                alert("Something error in the delete");
            }
        });
    }
}
function TopPost() {
    $.ajax({
        url: "Home/TopRecentPosts",
        type: 'GET',
        success: function (result) {
            $('#top-posts').html(result);
        },
        error: function () {
            console.log('Error loading top posts.');
        }
    });
}
function TopPopularPost() {
    $.ajax({
        url: "Home/TopPopularPosts",
        type: 'GET',
        success: function (result) {
            $('#top-popular-posts').html(result);
        },
        error: function () {
            console.log('Error loading top posts.');
        }
    });
}
