// Função para rolar para o final do container de mensagens
function scrollToBottom(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}

// Alternar tema escuro/claro
function toggleDarkMode() {
    const body = document.body;
    body.classList.toggle('dark-mode');
    
    // Salvar preferência no localStorage
    const isDarkMode = body.classList.contains('dark-mode');
    localStorage.setItem('darkModeEnabled', isDarkMode);
}

// Verificar tema preferido ao carregar a página
document.addEventListener('DOMContentLoaded', () => {
    const prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const savedMode = localStorage.getItem('darkModeEnabled');
    
    // Se houver uma preferência salva, use-a, caso contrário use a preferência do sistema
    if (savedMode !== null) {
        document.body.classList.toggle('dark-mode', savedMode === 'true');
    } else if (prefersDarkMode) {
        document.body.classList.add('dark-mode');
    }
});