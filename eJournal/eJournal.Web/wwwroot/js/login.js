
var lastUserEmail = localStorage.getItem("LastUserEmail");
var lastUserImage = localStorage.getItem("LastUserImage");
var lastUsername = localStorage.getItem("LastUsername");

if (lastUserEmail !== null) {
    document.getElementById("userEmail").innerText = lastUserEmail;
    document.getElementById("userImage").src = lastUserImage;
    document.getElementById("username").innerText = lastUsername;
    document.getElementById("welcome-message").innerText = "Welcome Back to Our eJournal";
   
}
else {
    document.getElementById("userEmail").innerText = '';
    document.getElementById("username").innerText = '';
}
