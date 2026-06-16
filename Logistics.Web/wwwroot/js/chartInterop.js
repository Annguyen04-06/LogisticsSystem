window.logisticsCharts = window.logisticsCharts || {};

window.renderBarChart = (canvasId, labels, data, label) => {
    renderChart(canvasId, "bar", labels, data, label);
};

window.renderLineChart = (canvasId, labels, data, label) => {
    renderChart(canvasId, "line", labels, data, label);
};

window.renderDoughnutChart = (canvasId, labels, data) => {
    renderChart(canvasId, "doughnut", labels, data, "");
};

function renderChart(canvasId, type, labels, data, label) {
    const canvas = document.getElementById(canvasId);

    if (!canvas || !window.Chart) {
        return;
    }

    if (window.logisticsCharts[canvasId]) {
        window.logisticsCharts[canvasId].destroy();
    }

    const colors = [
        "#2563eb",
        "#7c3aed",
        "#16a34a",
        "#f59e0b",
        "#dc2626",
        "#0891b2",
        "#4f46e5",
        "#0f766e"
    ];

    window.logisticsCharts[canvasId] = new Chart(canvas, {
        type,
        data: {
            labels,
            datasets: [{
                label,
                data,
                borderColor: "#2563eb",
                backgroundColor: type === "doughnut"
                    ? colors
                    : "rgba(37, 99, 235, 0.18)",
                borderWidth: 2,
                fill: type === "line",
                tension: 0.35
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: type === "doughnut"
                }
            },
            scales: type === "doughnut"
                ? {}
                : {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            precision: 0
                        }
                    }
                }
        }
    });
}
