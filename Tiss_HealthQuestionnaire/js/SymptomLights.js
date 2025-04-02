function updateIndicators() {
    document.querySelectorAll('.score-option').forEach((radio) => {
        radio.addEventListener('change', function () {
            let indicatorSelector = this.getAttribute('data-indicator');
            let indicator = document.querySelector(indicatorSelector);

            if (!indicator) {
                console.warn(`未找到燈號元素: ${indicatorSelector}`);
                return;
            }

            const score = parseInt(this.value);
            indicator.className = 'light-indicator';

            if (score === 0 || score === 1) {
                indicator.classList.add('light-green');
            } else if (score >= 2 && score <= 4) {
                indicator.classList.add('light-yellow');
            } else if (score >= 5 && score <= 6) {
                indicator.classList.add('light-red');
            }
        });
    });
}
document.addEventListener('DOMContentLoaded', function () {
    updateIndicators();
});