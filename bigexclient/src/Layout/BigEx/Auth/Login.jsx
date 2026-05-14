import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";

const Login = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        uEmail: "",
        uPassword: "",
    });

    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setLoading(true);

        try {
            // Отправляем данные на бэкенд
            const response = await axios.post(`${process.env.REACT_APP_API_URL}/Auth/login`, formData);

            // Теперь бэкенд возвращает объект с userId и email (без токена)
            if (response.data && response.data.userId) {
                // Сохраняем ID пользователя и email
                localStorage.setItem("userId", response.data.userId);
                localStorage.setItem("userEmail", response.data.email);

                alert("Вход выполнен успешно!");
                navigate("/");
            }
        } catch (err) {
            if (err.response && err.response.status === 401) {
                setError("Неверный email или пароль.");
            } else {
                setError("Ошибка сервера. Проверьте подключение к бэкенду.");
            }
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="py-12 flex items-center justify-center">
            <div className="bg-white p-8 rounded-2xl shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-extrabold text-center text-gray-800 mb-8">
                    Вход в BigEx
                </h2>

                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-2 rounded mb-4 text-sm">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Email</label>
                        <input
                            name="uEmail"
                            type="email"
                            required
                            value={formData.uEmail}
                            onChange={handleChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition"
                            placeholder="example@mail.com"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Пароль</label>
                        <input
                            name="uPassword"
                            type="password"
                            required
                            value={formData.uPassword}
                            onChange={handleChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition"
                            placeholder="********"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={loading}
                        className={`w-full bg-blue-600 text-white font-bold py-3 rounded-lg transition-all shadow-lg mt-4 
                            ${loading ? "opacity-50 cursor-not-allowed" : "hover:bg-blue-700 active:scale-[0.98]"}`}
                    >
                        {loading ? "Вход..." : "Войти"}
                    </button>
                </form>

                <p className="mt-8 text-center text-sm text-gray-600">
                    Нет аккаунта?{" "}
                    <Link to="/register" className="text-blue-600 font-bold hover:underline">
                        Зарегистрироваться
                    </Link>
                </p>
            </div>
        </div>
    );
};

export default Login;