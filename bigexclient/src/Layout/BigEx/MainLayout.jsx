import React from "react";
import { Outlet, Link } from "react-router-dom";

const MainLayout = () => {
    return (
        <div className="flex flex-col min-h-screen bg-gray-50 font-sans">
            {/* Шапка (Header) */}
            <header className="sticky top-0 z-50 bg-white/80 backdrop-blur-md border-b border-gray-200 py-4 px-6 shadow-sm">
                <div className="max-w-7xl mx-auto flex justify-between items-center">
                    <Link to="/" className="text-2xl font-bold text-blue-600 tracking-tight">
                        BigEx<span className="text-gray-800">Messenger</span>
                    </Link>
                    <nav className="space-x-4 text-sm font-medium text-gray-600">
                        <Link to="/login" className="hover:text-blue-600">Вход</Link>
                        <Link to="/register" className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition">
                            Регистрация
                        </Link>
                    </nav>
                </div>
            </header>

            {/* Контент (Content) — сюда будут подставляться страницы */}
            <main className="flex-grow">
                {/*Outlet — это место, где появится содержимое Login или Register */}
                <Outlet />
            </main>

            {/* Подвал (Footer) */}
            <footer className="bg-white border-t border-gray-200 py-6 text-center text-sm text-gray-500">
                <p>&copy; 2026 BigEx Project. Разработано с использованием .NET и React.</p>
            </footer>
        </div>
    );
};

export default MainLayout;