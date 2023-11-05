
$('#upload-button').click(function () {
    $('#image-input').click();
});

function previewImage(event) {
    var input = event.target;
    var reader = new FileReader();
    reader.onload = function () {
        var dataURL = reader.result;
        $('#profile-picture').attr('src', dataURL);
    };
    reader.readAsDataURL(input.files[0]);
}

function removeImage() {
    $('#profile-picture').attr('src', '/Images/user.png');
    $('#image-input').val(null);
    $('#image-path').val(null);
}

$("#department").on("keyup", function () {
    $("#department-error").hide();
});

$("#user-name").on("keyup", function () {
    $("#user-name-error").hide();
});

$("#first-name").on("keyup", function () {
    $("#first-name-error").hide();
});

$("#last-name").on("keyup", function () {
    $("#last-name-error").hide();
});

$("#designation").on("keyup", function () {
    $("#designation-error").hide();
});

$("#gender").on("keyup", function () {
    $("#gender-error").hide();
});

$("#phone").on("keyup", function () {
    $("#phone-error").hide();
});
$("#DateOfBirth").on("keyup", function () {
    $("#DateOfBirth-error").hide();
});