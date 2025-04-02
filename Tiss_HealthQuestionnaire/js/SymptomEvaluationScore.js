function updateSymptomsScores() {
    let totalSymptoms = 0;
    let totalScore = 0;

    document.querySelectorAll('.score-option:checked').forEach((input) => {
        let score = parseInt(input.value) || 0;
        if (score > 0) {
            totalSymptoms++;
        }
        totalScore += score;
    });

    document.getElementById('TotalSymptoms').textContent = totalSymptoms;
    document.getElementById('TotalScore').textContent = totalScore;
}

document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('change', function (event) {
        if (event.target.classList.contains('score-option')) {
            updateSymptomsScores();
        }
    });

    updateSymptomsScores();
});