document.addEventListener('DOMContentLoaded', function() {
    function toggleTags(button) {
        const hiddenTagsContainer = button.nextElementSibling;
        const isHidden = hiddenTagsContainer.style.maxHeight === '0px' || hiddenTagsContainer.style.maxHeight === '';
        const tagsCount = button.getAttribute('data-count');

        if (isHidden) {
            // Показываем теги
            hiddenTagsContainer.style.maxHeight = hiddenTagsContainer.scrollHeight + 'px';
            hiddenTagsContainer.style.opacity = '1';
            button.textContent = '⮝';
        } else {
            // Скрываем теги
            hiddenTagsContainer.style.maxHeight = '0px';
            hiddenTagsContainer.style.opacity = '0';
            button.textContent = '+' + tagsCount;
        }
    }

    // Находим все кнопки для показа/скрытия тегов
    const showMoreButtons = document.querySelectorAll('.show-more-tags');
    showMoreButtons.forEach(button => {
        button.addEventListener('click', () => toggleTags(button));
    });
}); 