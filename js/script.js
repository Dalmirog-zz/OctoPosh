function getReleases() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function(response) {
		var releasesJSON = createReleasesJSON(response);
		displayLatestRelease(releasesJSON);
		populateReleasesTable(releasesJSON);
	});
}

function displayLatestRelease(releasesJSON) {
	var version = document.getElementById("version");
	var latest = releasesJSON.length - 1;
	version.innerHTML = releasesJSON[latest].name;	
}

function createReleasesJSON(releasesResponse) {
	var releasesJSON = [];
	releasesResponse.forEach(function(item) {
		releasesJSON.push(item);
	});
	return releasesJSON;
}

function populateReleasesTable(releasesJSON) {
	var template = _.template($("#releases-template").html());
	releasesJSON.forEach(function(release) {
		$("#releases > tbody").append(template(release));
	});
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

$(document).ready(function() {
	getReleases();
	initZeroClipboard();
});