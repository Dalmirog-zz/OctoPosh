var githubReleases;

function getReleases() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function(response) {
		githubReleases = response;
		displayLatestRelease();
		populateReleasesTable();
	});
}

function displayLatestRelease() {
	var version = document.getElementById("version");
	var latest = githubReleases.length - 1;
	version.innerHTML = githubReleases[0].name;	
}

function initZeroClipboard() {
	var client = new ZeroClipboard( document.getElementById("copy-button"), {
		moviePath: "/js/ZeroClipboard.swf"
	});

	client.on("aftercopy", function(event) {
	    $("#copy-button > span").removeClass("glyphicon-copy");
	    $("#copy-button > span").addClass("glyphicon-ok");
	    $("#copy-button").removeClass("btn-primary");
	    $("#copy-button").addClass("btn-success");
	  });
}


function createReleasesJSON() {
	var releasesJSON = [];
	for (var i = 0; i <= 4; i++) {
		releasesJSON[i] = {};
		releasesJSON[i].name = "OctoPosh " + githubReleases[0].name; //Change [0] for something nice when I have more than one GH release.
		releasesJSON[i].date = githubReleases[0].published_at;
		releasesJSON[i].url = githubReleases[0].body;
	};
	return releasesJSON;
}

function populateReleasesTable() {
	JSON = createReleasesJSON();
	console.log(JSON);
	var template = _.template($("#releases-template").html());
	JSON.forEach(function(release) {
		$("#releases > tbody").append(template(release));
	});
}

$(document).ready(function() {
	getReleases();
	initZeroClipboard();
});