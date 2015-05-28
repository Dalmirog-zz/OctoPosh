/* Latest Releases Table */

// Get the data from GitHub API
function getReleases() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function callback(response) {
		var json = createReleasesJSON(extractLatestReleases(response));

		populateReleasesTable(json);
	});
}

// Extract the latest 5 releases
function extractLatestReleases(releasesResponse) {
	if (releasesResponse.length > 5) {
		releasesResponse.splice(5, releasesResponse.length);
	}
	return releasesResponse;
}

// Extract the necesary fields
function createReleasesJSON(releasesResponse) {
	var releasesJSON = [];

	releasesResponse.forEach(function each(item) {
		releasesJSON.push({
			name: item.name,
			url: item.html_url,
			date: formatDate(item.published_at),
			rlsNotes: item.body
		});
	});
	return releasesJSON;
}

// Format date to mm-dd-yyyy
function formatDate(dateString) {
	var date = new Date(dateString.substr(0,10));
	var formattedDate = [
		date.getMonth() + 1,
		date.getDate() + 1,
		date.getFullYear()
	];
	formattedDate = formattedDate.join("-");
	return formattedDate;
}

// Apply template for each one of the latest 5 releases
function populateReleasesTable(releasesJSON) {
	var template = _.template($("#releases-template").html());
	releasesJSON.forEach(function createRow(release) {
		$("#releases > tbody").append(template(release));
	});
}

/* Copy on click */

function initZeroClipboard() {
	var copyButton = document.getElementById("copy-button");
	var client = new ZeroClipboard( document.getElementById("copy-button"), {
		moviePath: "/js/ZeroClipboard.swf"
	});

	client.on("aftercopy", function(event) {
	    copyButton.className = "btn btn-xs btn-success pull-right hidden-xs hidden-sm";
	    copyButton.blur();
	    document.getElementById("copy-icon").className = "glyphicon glyphicon-ok";
	  });
}

/* ----------------------------- */

$(document).ready(function() {
	getReleases();
	initZeroClipboard();
});