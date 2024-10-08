window.confettiInterop = {
    launchConfetti: function () {
        //left corner
        confetti({
            particleCount: 150,
            angle: 135,
            drift: 0,
            spread: 70,
            origin: {x: 1, y: 0.95},
            startVelocity: 90
        });

        //right corner
        confetti({
            particleCount: 185,
            angle: 45,
            drift: 0,
            spread: 70,
            origin: {x: 0, y: 0.95},
            startVelocity: 90
        });

        return true;
    }
};
