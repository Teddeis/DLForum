class ImageGallery {
    constructor() {
        this.currentIndex = 0;
        this.images = Array.from(document.querySelectorAll('.image-gallery img')).map(img => img.src);
        
        this.initializeControls();
    }

    initializeControls() {
        document.getElementById('prevButton')?.addEventListener('click', () => this.changeImage(-1));
        document.getElementById('nextButton')?.addEventListener('click', () => this.changeImage(1));
    }

    changeImage(direction) {
        this.currentIndex = (this.currentIndex + direction + this.images.length) % this.images.length;
        const currentImage = document.getElementById('currentImage');
        if (currentImage) {
            currentImage.src = this.images[this.currentIndex];
        }
    }
}

document.addEventListener('DOMContentLoaded', () => new ImageGallery()); 