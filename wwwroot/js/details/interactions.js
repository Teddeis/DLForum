class Interactions {
    constructor() {
        this.initializeLikes();
        this.initializeFavorites();
    }

    initializeLikes() {
        document.querySelectorAll('.like-form').forEach(form => {
            form.addEventListener('submit', async (e) => {
                e.preventDefault();
                const response = await this.submitForm(form);
                
                if (response.error === "NotLoggedIn") {
                    window.location.href = "/Account/Login";
                    return;
                }

                this.updateLikeUI(form, response);
            });
        });
    }

    initializeFavorites() {
        document.querySelectorAll('.favorite-form').forEach(form => {
            form.addEventListener('submit', async (e) => {
                e.preventDefault();
                const response = await this.submitForm(form);
                this.updateFavoriteUI(form, response);
            });
        });
    }

    async submitForm(form) {
        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: new FormData(form)
            });
            return await response.json();
        } catch (error) {
            console.error('Ошибка при отправке формы:', error);
            return null;
        }
    }

    updateLikeUI(form, data) {
        const button = form.querySelector('.like-btn i');
        const counter = form.querySelector('.like-count');
        
        if (data.isLike) {
            button.classList.replace('bi-heart', 'bi-heart-fill');
            button.style.color = 'red';
        } else {
            button.classList.replace('bi-heart-fill', 'bi-heart');
            button.style.color = 'var(--text-color)';
        }
        
        counter.textContent = data.likeCount;
    }

    updateFavoriteUI(form, data) {
        const button = form.querySelector('.favorite-btn i');
        
        if (data.isFavorite) {
            button.classList.replace('bi-bookmark', 'bi-bookmark-fill');
            button.style.color = 'gold';
        } else {
            button.classList.replace('bi-bookmark-fill', 'bi-bookmark');
            button.style.color = 'var(--text-color)';
        }
    }
}

document.addEventListener('DOMContentLoaded', () => new Interactions()); 