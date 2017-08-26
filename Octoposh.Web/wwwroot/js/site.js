var btn = document.getElementById('copy-button');
var clipboard = new Clipboard(btn);
clipboard.on('success', function (e) {
    console.log(e);
});
clipboard.on('error', function (e) {
    console.log(e);
});