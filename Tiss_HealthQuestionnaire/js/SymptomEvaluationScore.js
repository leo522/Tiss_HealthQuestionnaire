///** 更新症狀自我評估的分數邏輯 **/
//function updateSymptomsScores() {
//    let totalSymptoms = 0;  // 計算填 1 分以上的項目數量
//    let totalScore = 0;      // 計算所有症狀的分數總和

//    document.querySelectorAll('.score-option:checked').forEach((input) => {
//        let score = parseInt(input.value) || 0;
//        if (score > 0) {
//            totalSymptoms++; // 只計算 1 分以上的項目數量
//        }
//        totalScore += score; // 累加所有症狀的分數
//    });

//    // 更新 DOM
//    document.getElementById('TotalSymptoms').textContent = totalSymptoms;
//    document.getElementById('TotalScore').textContent = totalScore;
//}

///** 監聽症狀選擇變化 **/
//document.querySelectorAll('.score-option').forEach(input => {
//    input.addEventListener('change', updateSymptomsScores);
//});

///** 頁面加載時初始化分數 **/
//document.addEventListener('DOMContentLoaded', updateSymptomsScores);
function updateSymptomsScores() {
    let totalSymptoms = 0;  // 計算填 1 分以上的項目數量
    let totalScore = 0;      // 計算所有症狀的分數總和

    document.querySelectorAll('.score-option:checked').forEach((input) => {
        let score = parseInt(input.value) || 0;
        if (score > 0) {
            totalSymptoms++; // 只計算 1 分以上的項目數量
        }
        totalScore += score; // 累加所有症狀的分數
    });

    // 更新分數顯示
    document.getElementById('TotalSymptoms').textContent = totalSymptoms;
    document.getElementById('TotalScore').textContent = totalScore;
}

/** 監聽所有 radio 按鈕的變化 **/
document.addEventListener('DOMContentLoaded', function () {
    document.addEventListener('change', function (event) {
        if (event.target.classList.contains('score-option')) {
            updateSymptomsScores();
        }
    });

    // 確保頁面載入時立即更新一次
    updateSymptomsScores();
});