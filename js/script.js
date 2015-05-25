var version = document.getElementById("version");

$(document).ready(function() {
	$.getJSON("https://raw.githubusercontent.com/anabellaspinelli/OctoPosh/gh-pages/version.json", function(response) {
		version.innerHTML = "v." + response.version.number;
	});
});
