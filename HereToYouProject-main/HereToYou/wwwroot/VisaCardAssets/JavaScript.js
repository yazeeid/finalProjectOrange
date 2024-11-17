function showLoading(event, button) {
    event.preventDefault(); // Prevent form submission

    button.innerHTML = "Processing Payment...";

    setTimeout(function () {
        button.innerHTML = "Payment completed.";
    }, 3000); // Change to the desired duration in milliseconds
}
function validateForm() {
    // Clear previous error messages
    document.getElementById("card-number-error").textContent = "";
    document.getElementById("card-holder-error").textContent = "";
    document.getElementById("expiry-date-error").textContent = "";
    document.getElementById("cvv-error").textContent = "";

    // Get form values
    const cardNumber = document.getElementById("card-number").value.trim();
    const cardHolder = document.getElementById("card-holder").value.trim();
    const expiryDate = document.getElementById("expiry-date").value.trim();
    const cvv = document.getElementById("cvv").value.trim();

    // Check if fields are empty
    if (cardNumber === "") {
        document.getElementById("card-number-error").textContent = "Card Number is required.";
        return false;
    }
    if (cardHolder === "") {
        document.getElementById("card-holder-error").textContent = "Card Holder is required.";
        return false;
    }
    if (expiryDate === "") {
        document.getElementById("expiry-date-error").textContent = "Expiry Date is required.";
        return false;
    }
    if (cvv === "") {
        document.getElementById("cvv-error").textContent = "CVV is required.";
        return false;
    }

    // Validate card number
    const cardNumberRegex = /^[0-9]{16}$/;
    if (!cardNumberRegex.test(cardNumber)) {
        document.getElementById("card-number-error").textContent = "Invalid Card Number. Must be 16 digits.";
        return false;
    }

    // Validate expiry date
    const expiryDateRegex = /^(0[1-9]|1[0-2])\/?([0-9]{2})$/;
    if (!expiryDateRegex.test(expiryDate)) {
        document.getElementById("expiry-date-error").textContent = "Invalid Expiry Date. Format MM/YY.";
        return false;
    }

    // Validate CVV
    const cvvRegex = /^[0-9]{3,4}$/;
    if (!cvvRegex.test(cvv)) {
        document.getElementById("cvv-error").textContent = "Invalid CVV. Must be 3 or 4 digits.";
        return false;
    }

    // If all validations pass
    return true;
}
