import React, { useState, useEffect } from "react";
import { useParams, useLocation, useNavigate } from "react-router-dom";
import axios from "axios";

const Chat = () => {
    const { id } = useParams();
    const location = useLocation();
    const navigate = useNavigate();

    const [messages, setMessages] = useState([]);
    const [text, setText] = useState("");
    const [chatInfo, setChatInfo] = useState(location.state?.recipient || null);
    const userId = localStorage.getItem("userId");

    const sendMessage = async (e) => {
        e.preventDefault();
        if (!text.trim()) return;

        try {
            if (id === "new" && location.state?.isNew) {
                // ЛОГИКА ПЕРВОГО СООБЩЕНИЯ
                const response = await axios.post("http://localhost:5000/api/IChat/create-and-send", {
                    senderId: userId,
                    recipientEmail: chatInfo.uEmail,
                    messageText: text
                });

                // После создания чата бэкенд вернет новый ID чата
                const newChatId = response.data.ichatPublicId;
                navigate(`/chat/${newChatId}`, { replace: true });
            } else {
                // Обычная отправка в существующий чат
                await axios.post(`http://localhost:5000/api/IChat/message`, {
                    chatId: id,
                    senderId: userId,
                    text: text
                });
            }
            setText("");
            // Тут логика обновления сообщений (через SignalR или повторный запрос)
        } catch (err) {
            console.error("Ошибка отправки:", err);
        }
    };

    return (
        <div className="flex flex-col h-[80vh] max-w-2xl mx-auto bg-white shadow-lg rounded-2xl overflow-hidden">
            {/* Хедер чата */}
            <div className="p-4 border-b bg-gray-50 flex items-center">
                <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white font-bold">
                    {chatInfo?.uName ? chatInfo.uName[0].toUpperCase() : "?"}
                </div>
                <div className="ml-3">
                    <h2 className="font-bold">{chatInfo?.uName || "Загрузка..."}</h2>
                    <p className="text-xs text-green-500">онлайн</p>
                </div>
            </div>

            {/* Окно сообщений */}
            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-100">
                {messages.map((m, i) => (
                    <div key={i} className={`flex ${m.senderId == userId ? "justify-end" : "justify-start"}`}>
                        <div className={`p-3 rounded-2xl max-w-[80%] ${m.senderId == userId ? "bg-blue-600 text-white" : "bg-white text-gray-800 shadow-sm"}`}>
                            {m.text}
                        </div>
                    </div>
                ))}
                {id === "new" && messages.length === 0 && (
                    <p className="text-center text-gray-400 text-sm mt-10">Напишите что-нибудь, чтобы начать общение</p>
                )}
            </div>

            {/* Поле ввода */}
            <form onSubmit={sendMessage} className="p-4 border-t bg-white flex gap-2">
                <input
                    type="text"
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    placeholder="Ваше сообщение..."
                    className="flex-1 p-2 border rounded-xl outline-none focus:ring-2 focus:ring-blue-500"
                />
                <button className="bg-blue-600 text-white px-6 py-2 rounded-xl hover:bg-blue-700 transition">
                    Отправить
                </button>
            </form>
        </div>
    );
};

export default Chat;