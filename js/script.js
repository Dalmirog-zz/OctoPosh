var version = document.getElementById("version");

$(document).ready(function() {
	$.getJSON("https://api.mercadolibre.com/sites/MLA/search?q=bicicleta+de+paseo+rod+28", function(response) {
		version.innerHTML = "v." + response.paging.total;
	});
});