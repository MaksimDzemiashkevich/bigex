import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";

const Register = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        uName: "",
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
            const response = await axios.post(`${process.env.REACT_APP_API_URL}/Auth/signin`, formData);

            if (response.data === true) {
                alert("Регистрация успешна!");
                navigate("/login");
            } else {
                setError("Ошибка при регистрации. Возможно, email уже занят.");
            }
        } catch (err) {
            setError("Ошибка сервера. Проверьте подключение к бэкенду.");
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="py-10 flex items-center justify-center">
            <div className="bg-white p-8 rounded-2xl shadow-xl w-full max-w-md">
                <h2 className="text-3xl font-extrabold text-center text-gray-800 mb-8">
                    Создать аккаунт
                </h2>

                {error && (
                    <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-2 rounded mb-4 text-sm">
                        {error}
                    </div>
                )}

                <form onSubmit={handleSubmit} className="space-y-5">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Имя</label>
                        <input
                            name="uName"
                            type="text"
                            required
                            value={formData.uName}
                            onChange={handleChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition"
                            placeholder="Александр"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Email адрес</label>
                        <input
                            name="uEmail"
                            type="email"
                            required
                            value={formData.uEmail}
                            onChange={handleChange}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition"
                            placeholder="mail@example.com"
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
                            placeholder="••••••••"
                        />
                    </div>

                    <button
                        type="submit"
                        disabled={loading}
                        className={`w-full bg-blue-600 text-white font-bold py-3 rounded-lg transition-all shadow-lg mt-4 
                            ${loading ? "opacity-50 cursor-not-allowed" : "hover:bg-blue-700 active:scale-[0.98]"}`}
                    >
                        {loading ? "Регистрация..." : "Зарегистрироваться"}
                    </button>
                </form>

                <div className="mt-8 text-center text-sm text-gray-600">
                    Уже есть аккаунт?{" "}
                    <Link to="/login" className="text-blue-600 font-bold hover:underline">
                        Войти
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default Register;