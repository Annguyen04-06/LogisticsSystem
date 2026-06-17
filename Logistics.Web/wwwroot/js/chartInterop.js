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

    const ctx = canvas.getContext("2d");
    const primaryGradient = ctx.createLinearGradient(0, 0, 0, canvas.clientHeight || 320);
    primaryGradient.addColorStop(0, "rgba(37, 99, 235, 0.92)");
    primaryGradient.addColorStop(0.55, "rgba(79, 70, 229, 0.66)");
    primaryGradient.addColorStop(1, "rgba(124, 58, 237, 0.28)");

    const softLineGradient = ctx.createLinearGradient(0, 0, 0, canvas.clientHeight || 320);
    softLineGradient.addColorStop(0, "rgba(37, 99, 235, 0.26)");
    softLineGradient.addColorStop(1, "rgba(124, 58, 237, 0.03)");

    const colors = [
        "#2563eb",
        "#7c3aed",
        "#16a34a",
        "#f59e0b",
        "#dc2626",
        "#0891b2",
        "#4f46e5",
        "#0f766e",
        "#db2777",
        "#9333ea"
    ];

    const isCurrencyChart = label && label.toLowerCase().includes("doanh thu");

    window.logisticsCharts[canvasId] = new Chart(canvas, {
        type,
        data: {
            labels,
            datasets: [{
                label,
                data,
                borderColor: type === "line" ? "#2563eb" : "rgba(255, 255, 255, 0.92)",
                backgroundColor: type === "doughnut"
                    ? colors
                    : type === "line"
                        ? softLineGradient
                        : primaryGradient,
                borderWidth: type === "doughnut" ? 4 : 2,
                borderRadius: type === "bar" ? 12 : 0,
                borderSkipped: false,
                fill: type === "line",
                pointBackgroundColor: "#ffffff",
                pointBorderColor: "#2563eb",
                pointBorderWidth: 3,
                pointHoverRadius: 7,
                pointRadius: type === "line" ? 4 : 0,
                tension: 0.38
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            animation: {
                duration: 850,
                easing: "easeOutQuart"
            },
            interaction: {
                intersect: false,
                mode: "index"
            },
            plugins: {
                legend: {
                    display: type === "doughnut",
                    position: "bottom",
                    labels: {
                        boxWidth: 12,
                        color: "#475569",
                        font: {
                            family: "Inter, Segoe UI, Arial, sans-serif",
                            weight: "700"
                        },
                        padding: 18,
                        usePointStyle: true
                    }
                },
                tooltip: {
                    backgroundColor: "rgba(15, 23, 42, 0.94)",
                    bodyFont: {
                        family: "Inter, Segoe UI, Arial, sans-serif",
                        weight: "700"
                    },
                    borderColor: "rgba(255, 255, 255, 0.18)",
                    borderWidth: 1,
                    callbacks: {
                        label: context => {
                            const value = Number(context.raw || 0);
                            const text = isCurrencyChart
                                ? formatVnd(value)
                                : value.toLocaleString("vi-VN");
                            const datasetLabel = context.dataset.label ? `${context.dataset.label}: ` : "";
                            return `${datasetLabel}${text}`;
                        }
                    },
                    cornerRadius: 12,
                    displayColors: true,
                    padding: 12,
                    titleFont: {
                        family: "Inter, Segoe UI, Arial, sans-serif",
                        weight: "800"
                    }
                }
            },
            cutout: type === "doughnut" ? "64%" : undefined,
            scales: type === "doughnut"
                ? {}
                : {
                    x: {
                        grid: {
                            display: false
                        },
                        ticks: {
                            color: "#64748b",
                            font: {
                                family: "Inter, Segoe UI, Arial, sans-serif",
                                weight: "700"
                            }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: "rgba(148, 163, 184, 0.18)",
                            drawBorder: false
                        },
                        ticks: {
                            color: "#64748b",
                            precision: 0,
                            callback: value => isCurrencyChart
                                ? compactVnd(value)
                                : Number(value).toLocaleString("vi-VN")
                        }
                    }
                }
        }
    });
}

function formatVnd(value) {
    return `${Number(value).toLocaleString("vi-VN")} đ`;
}

function compactVnd(value) {
    const number = Number(value);

    if (number >= 1000000000) {
        return `${(number / 1000000000).toLocaleString("vi-VN", { maximumFractionDigits: 1 })} tỷ`;
    }

    if (number >= 1000000) {
        return `${(number / 1000000).toLocaleString("vi-VN", { maximumFractionDigits: 1 })} tr`;
    }

    return number.toLocaleString("vi-VN");
}
