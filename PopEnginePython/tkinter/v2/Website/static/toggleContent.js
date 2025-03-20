function toggleFields(checkboxId, fieldId) {
    // Get the checkbox and the field to toggle
    var checkbox = document.getElementById(checkboxId);
    var field = document.getElementById(fieldId);
    
    // Show or hide the field based on the checkbox state
    if (checkbox.checked) {
        field.style.display = "block"; // Show the field
    } else {
        field.style.display = "none"; // Hide the field
    }
}