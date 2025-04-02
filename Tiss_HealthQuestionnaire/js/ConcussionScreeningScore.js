function updateScores() {
    const scores = {
        Orientation: sumScores('input[name^="Orientation_"]:checked'),
        ImmediateMemoryFirst: sumScores('input[name^="ImmediateMemory_first_"]:checked'),
        ImmediateMemorySecond: sumScores('input[name^="ImmediateMemory_second_"]:checked'),
        ImmediateMemoryThird: sumScores('input[name^="ImmediateMemory_third_"]:checked'),
        Concentration: sumScores('input[name^="Concentration_"]:checked'),
        CoordinationErrors: sumScores('.error-input'),
        DelayedRecall: sumScores('input[name^="DelayedRecall_"]:checked'),
    };

    const totalImmediateMemory = scores.ImmediateMemoryFirst + scores.ImmediateMemorySecond + scores.ImmediateMemoryThird;

    const totalErrors = scores.CoordinationErrors;
    const coordinationScore = 30 - totalErrors;

    updateDOMScoreWithMax('OrientationTotalScore', scores.Orientation, 5);
    updateDOMScoreWithMax('ImmediateMemoryFirstTotal', scores.ImmediateMemoryFirst, 10);
    updateDOMScoreWithMax('ImmediateMemorySecondTotal', scores.ImmediateMemorySecond, 10);
    updateDOMScoreWithMax('ImmediateMemoryThirdTotal', scores.ImmediateMemoryThird, 10);
    updateDOMScoreWithMax('ImmediateMemoryTotalScore', totalImmediateMemory, 30);
    updateDOMScoreWithMax('ConcentrationTotalScore', scores.Concentration, 4);
    updateDOMScoreWithMax('Coordination_TotalErrors', totalErrors, 30);

    if (totalErrors > 30) {
        Swal.fire({
            icon: 'error',
            title: '錯誤總數超過 30',
            text: '請檢查並重新填寫錯誤次數！',
        });
    }

    const firstTime = parseFloat(document.querySelector('input[name="Coordination_FirstTime"]').value) || 0;
    const secondTime = parseFloat(document.querySelector('input[name="Coordination_SecondTime"]').value) || 0;
    const thirdTime = parseFloat(document.querySelector('input[name="Coordination_ThirdTime"]').value) || 0;

    let times = [firstTime, secondTime, thirdTime].filter(time => time > 0);
    let averageTime = times.length > 0 ? (times.reduce((a, b) => a + b, 0) / times.length).toFixed(2) : '0.00';
    let fastestTime = times.length > 0 ? Math.min(...times).toFixed(2) : '0.00';

    document.getElementById('Coordination_AverageTime').textContent = `${averageTime} 秒`;
    document.getElementById('Coordination_FastestTime').textContent = `${fastestTime} 秒`;

    updateDOMScoreWithMax('DelayedRecallTotalScore', scores.DelayedRecall, 10);
    updateDOMScoreWithMax('FinalScore', scores.Orientation + totalImmediateMemory + scores.Concentration + coordinationScore + scores.DelayedRecall, 79);

    sessionStorage.setItem('ImmediateMemoryFirstTotal', scores.ImmediateMemoryFirst);
    sessionStorage.setItem('ImmediateMemorySecondTotal', scores.ImmediateMemorySecond);
    sessionStorage.setItem('ImmediateMemoryThirdTotal', scores.ImmediateMemoryThird);
    sessionStorage.setItem('ImmediateMemoryTotalScore', totalImmediateMemory);
    updateIndicators();
}

function sumScores(selector) {
    let total = 0;
    document.querySelectorAll(selector).forEach((input) => {
        total += parseInt(input.value) || 0;
    });
    return total;
}

function updateDOMScoreWithMax(id, value, max) {
    const element = document.getElementById(id);
    if (!element) return;

    if (element.textContent.includes('/')) {
        let parts = element.textContent.split('/');
        element.textContent = `${value} / ${parts[1].trim()}`;
    } else {
        element.textContent = value;
    }
}

document.querySelectorAll('input[name^="Coordination_"]').forEach(input => {
    input.addEventListener('input', updateScores);
});

const inputSelectors = [
    'input[name^="Orientation_"]',
    'input[name^="ImmediateMemory_"]',
    '.concentration-option',
    '.error-input',
    '.recall-option',
];
inputSelectors.forEach((selector) => {
    document.querySelectorAll(selector).forEach((input) => {
        input.addEventListener('change', updateScores);
    });
});

updateScores();