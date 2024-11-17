let slideIndex = 0;
const slidesToShow = 4; // Number of slides to show at a time
const slides = document.getElementsByClassName("slide");
const totalSlides = slides.length;

// Set the width of the slider to 110%
document.querySelector(".slider").style.width = `110%`;

const slideWidth = 110 / slidesToShow; // Adjusted percentage width of each slide

function plusSlides(n) {
    slideIndex = (slideIndex + n + totalSlides) % totalSlides;
    showSlides();
}

function showSlides() {
    const slider = document.querySelector(".slider");
    const offset = slideIndex * slideWidth;
    slider.style.transform = `translateX(-${offset}%)`;
}

// Event listeners for previous and next buttons
document.querySelector(".prev").addEventListener("click", () => plusSlides(-1));
document.querySelector(".next").addEventListener("click", () => plusSlides(1));

// Initial call to show the first set of slides
showSlides();
