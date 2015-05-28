/* Latest Releases Table */

// Get the data from GitHub API
function getReleases() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function(response) {
		var latestReleases = extractLatestReleases(response);
		var releasesJSON = createReleasesJSON(latestReleases);
		populateReleasesTable(releasesJSON);
	});
}

// Extract the latest 5 releases
function extractLatestReleases(releasesResponse) {
	if (releasesResponse.length > 5) {
		return response.splice(4, response.length);
	}
	return releasesResponse;
}

// Extract the necesary fields
function createReleasesJSON(releasesResponse) {
	var releasesJSON = [];
	releasesResponse.forEach(function(item) {
		var tag = {};
		tag.name = item.name;
		tag.url = item.html_url;
		tag.date = formatDate(item.published_at);
		releasesJSON.push(tag);
	});
	return releasesJSON;
}

// Format date to mm-dd-yyyy
function formatDate(dateString) {
	dateString = dateString.substr(0,10);
	var date = new Date(dateString);
	var formattedDate = [];
	formattedDate.push(date.getMonth() + 1);
	formattedDate.push(date.getDate() + 1);
	formattedDate.push(date.getFullYear());
	formattedDate = formattedDate.join("-");
	return formattedDate;
}

// Apply template for each one of the latest 5 releases
function populateReleasesTable(releasesJSON) {
	var template = _.template($("#releases-template").html());
	releasesJSON.forEach(function(release) {
		$("#releases > tbody").append(template(release));
	});
}

/* Copy on click */

function initZeroClipboard() {
	var client = new ZeroClipboard( document.getElementById("copy-button"), {
		moviePath: "/js/ZeroClipboard.swf"
	});

	client.on("aftercopy", function(event) {
	    $("#copy-button > span").removeClass("glyphicon-copy");
	    $("#copy-button > span").addClass("glyphicon-ok");
	    $("#copy-button").removeClass("btn-primary");
	    $("#copy-button").addClass("btn-success");
	    $("#copy-button").blur();
	  });
}

/* ----------------------------- */

$(document).ready(function() {
	getReleases();
	initZeroClipboard();
});