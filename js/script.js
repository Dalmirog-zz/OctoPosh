
function getAndShowLatestVersion(versions) {
	var version = document.getElementById("version");
	var latest = versions.length - 1;
	version.innerHTML = versions[latest].name;
}

$(document).ready(function() {
	$.getJSON("https://api.github.com/repos/dalmirog/OctoPosh/releases", function(response) {
		getAndShowLatestVersion(response);
	});
	//ZeroClipboard.config( { moviePath: "https://cdnjs.cloudflare.com/ajax/libs/zeroclipboard/2.2.0/ZeroClipboard.swf" } );

	var client = new ZeroClipboard( document.getElementById("copy-button"), {
		moviePath: "/js/ZeroClipboard.swf"
	} );

	client.on( "ready", function( readyEvent ) {
	   alert( "ZeroClipboard SWF is ready!" );

	  client.on( "aftercopy", function( event ) {
	    // `this` === `client`
	    // `event.target` === the element that was clicked
	    event.target.style.display = "none";
	    alert("Copied text to clipboard: " + event.data["text/plain"] );
	  } );
	} );
});

// GET ALL RELEASES /repos/:owner/:repo/releases/

