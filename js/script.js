
function getAndShowLatestVersion(versions) {
	var version = document.getElementById("version");
	var latest = versions.length - 1;
	version.innerHTML = versions[latest].name;
}

$(document).ready(function() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function(response) {
		getAndShowLatestVersion(response);
	});
});

// GET ALL RELEASES /repos/:owner/:repo/releases/