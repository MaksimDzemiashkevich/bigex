import React, { useEffect, useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";

const Home = () => {
    const [chats, setChats] = useState([]);
    const [searchQuery, setSearchQuery] = useState("");
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchChats = async () => {
            const userId = localStorage.getItem("userId");

            if (!userId) {
                navigate("/login");
                return;
            }

            try {
                // Передаем userId в query string
                const response = await axios.get(`http://localhost:5000/api/IChat/ichats?userId=${userId}`);
                setChats(response.data);
            } catch (err) {
                console.error("Ошибка загрузки чатов:", err);
            } finally {
                setLoading(false);
            }
        };

        fetchChats();
    }, [navigate]);

    const handleSearch = async (e) => {
        const value = e.target.value;
        setSearchQuery(value);

        if (value.length < 1) {
            setSearchResults([]);
            return;
        }

        try {
            // Убрали headers с токеном, так как перешли на незащищенный поиск
            const response = await axios.get(`http://localhost:5000/api/IChat/search-users?query=${value}`);
            setSearchResults(response.data);
        } catch (err) {
            console.error("Ошибка поиска:", err);
        }
    };

    if (loading) return <div className="flex justify-center mt-10">Загрузка чатов...</div>;

    const startChat = (targetUser) => {
        // Проверяем, есть ли уже чат с этим пользователем в списке текущих чатов
        // (Для этого бэкенд должен возвращать информацию о собеседнике в списке чатов)
        const existingChat = chats.find(c => c.ichatName === targetUser.uName);

        if (existingChat) {
            navigate(`/chat/${existingChat.ichatPublicId}`);
        } else {
            // Если чата нет, переходим на временный роут (или тот же), 
            // но передаем данные пользователя в state
            navigate(`/chat/new`, {
                state: {
                    isNew: true,
                    recipient: targetUser
                }
            });
        }
    };

    return (
        <div className="max-w-2xl mx-auto py-8 px-4">
            {/* БЛОК ПОИСКА */}
            <div className="relative mb-10">
                <label className="block text-sm font-medium text-gray-700 mb-2">Найти новых людей</label>
                <input
                    type="text"
                    placeholder="Введите имя или email..."
                    className="w-full p-4 rounded-xl border border-gray-200 shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent outline-none transition-all"
                    value={searchQuery}
                    onChange={handleSearch}
                />

                {searchResults.length > 0 && (
                    <div className="absolute z-50 w-full mt-2 bg-white border border-gray-100 rounded-xl shadow-xl overflow-hidden">
                        <div className="p-2 bg-gray-50 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                            Найденные пользователи
                        </div>
                        {searchResults.map((user, idx) => (
                            <div
                                key={idx}
                                className="flex items-center justify-between p-4 hover:bg-blue-50 transition cursor-pointer border-b border-gray-50 last:border-none"
                            >
                                <div className="flex items-center">
                                    <div className="w-10 h-10 rounded-full bg-blue-600 flex items-center justify-center text-white font-bold">
                                        {user.uName ? user.uName[0].toUpperCase() : "U"}
                                    </div>
                                    <div className="ml-3">
                                        <p className="font-bold text-gray-800">{user.uName}</p>
                                        <p className="text-sm text-gray-500">{user.uEmail}</p>
                                    </div>
                                </div>
                                <button
                                    className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700 transition"
                                    onClick={() => startChat(user)}
                                >
                                    Написать
                                </button>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <hr className="mb-8 border-gray-100" />

            {/* СПИСОК ЧАТОВ */}
            <h1 className="text-2xl font-bold mb-6 text-gray-800">Ваши чаты</h1>

            {chats.length === 0 ? (
                <div className="text-center p-10 bg-gray-50 rounded-2xl border-2 border-dashed border-gray-200">
                    <p className="text-gray-400 italic">У вас пока нет активных диалогов</p>
                </div>
            ) : (
                <div className="space-y-3">
                    {chats.map((chat) => (
                        <div
                            key={chat.ichatPublicId}
                            className="flex items-center p-4 bg-white rounded-xl shadow-sm hover:shadow-md transition-all cursor-pointer border border-gray-100 group"
                            onClick={() => navigate(`/chat/${chat.ichatPublicId}`)}
                        >
                            <div className="w-14 h-14 rounded-full bg-gradient-to-tr from-blue-500 to-blue-400 flex items-center justify-center text-white text-xl font-bold shadow-inner overflow-hidden">
                                {chat.ichatMainImage ? (
                                    <img src={chat.ichatMainImage} alt="Avatar" className="w-full h-full object-cover" />
                                ) : (
                                    chat.ichatName ? chat.ichatName[0].toUpperCase() : "?"
                                )}
                            </div>

                            <div className="ml-4 flex-1">
                                <div className="flex justify-between items-start">
                                    <h3 className="font-bold text-gray-900 group-hover:text-blue-600 transition-colors">
                                        {chat.ichatName}
                                    </h3>
                                    <span className="text-[10px] bg-gray-100 text-gray-500 px-2 py-0.5 rounded-full uppercase font-bold tracking-tighter">
                                        {chat.chatType === 0 ? "Группа" : chat.chatType === 1 ? "Чат" : "Канал"}
                                    </span>
                                </div>
                                <p className="text-sm text-gray-500 truncate mt-1">
                                    {chat.ichatDescription || "Оставьте первое сообщение..."}
                                </p>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default Home;