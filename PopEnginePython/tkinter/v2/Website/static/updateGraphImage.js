var imageURI = 'static/images/plot.png';
var img = new Image();

// Function to update the canvas with the new image
function updateCanvas() {
    img.src = imageURI + '?d=' + Date.now(); // Cache busting

    var canvas = document.getElementById("graphCanvas");
    var context = canvas.getContext("2d");

    // Ensure image is fully loaded before drawing
    img.onload = function() {
        context.clearRect(0, 0, canvas.width, canvas.height);

        // Calculate the position to center the image in the canvas
        var x = (canvas.width - img.width) / 2;
        var y = (canvas.height - img.height) / 2;

        context.drawImage(img, x, y, img.width, img.height);
    };

    img.onerror = function() {
        console.error("Error loading image.");
    };
}

// Handle next time step
document.getElementById("nextTimeStepButton").addEventListener("click", function() {
    var loadingIndicator = document.getElementById("loadingIndicator");
    loadingIndicator.style.display = "block";

    fetch('/nextTimeStep', { method: 'POST' })
    .then(response => {
        if (response.ok) {
            updateCanvas();
        }
    })
    .catch(error => console.error('Error:', error))
    .finally(() => loadingIndicator.style.display = "none");
});

// Handle fast forward
document.getElementById("ffButton").addEventListener("click", function() {
    var ffAmount = document.getElementById("ffAmount").value;
    var loadingIndicator = document.getElementById("loadingIndicator");
    loadingIndicator.style.display = "block";

    fetch('/fastForward', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `ffAmount=${ffAmount}`,
    })
    .then(response => {
        if (response.ok) {
            updateCanvas();
        }
    })
    .catch(error => console.error('Error:', error))
    .finally(() => loadingIndicator.style.display = "none");
});

// Handle reset
document.getElementById("resetButton").addEventListener("click", function() {
    fetch('/reset', { method: 'POST' })
    .then(() => updateCanvas())
    .catch(error => console.error('Error:', error));
});
