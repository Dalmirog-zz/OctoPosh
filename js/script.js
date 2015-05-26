var version = document.getElementById("version");

$(document).ready(function() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/contents/SiteInfo.json?ref=development", function(response) {
		var versions = JSON.parse(window.atob(response.content).substr(3));
		var latest = versions.Items.length - 1;
		version.innerHTML = "v." + versions.Items[latest].Version;
	});
});