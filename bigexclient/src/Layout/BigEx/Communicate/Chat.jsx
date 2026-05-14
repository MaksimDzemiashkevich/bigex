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
    const [chatName, setChatName] = useState(
        location.state?.recipient?.uName || location.state?.recipient?.UName || null
    );
    const userId = Number(localStorage.getItem("userId"));

    useEffect(() => {
        const fetchMessages = async () => {
            if (id === "new") return;

            try {
                const response = await axios.get(`http://localhost:5000/api/IChat/messages?chatId=${id}`);
                console.log("Данные сообщений с бэкенда:", response.data);

                setMessages(response.data);
            } catch (err) {
                console.error("Ошибка загрузки сообщений:", err);
            }
        };

        fetchMessages();

        const interval = setInterval(fetchMessages, 3000);
        return () => clearInterval(interval);
    }, [id]);

    useEffect(() => {
        if (id === "new") return;

        const fetchChatInfo = async () => {
            try {
                const response = await axios.get(
                    `http://localhost:5000/api/IChat/chat-companion?chatId=${id}&userId=${userId}`
                );
                setChatName(response.data.uName || response.data.UName);
            } catch (err) {
                console.error("Ошибка загрузки собеседника:", err);
            }
        };
        if (!chatName) fetchChatInfo();
    }, [id]);

    const sendMessage = async (e) => {
        e.preventDefault();
        if (!text.trim()) return;

        try {
            if (id === "new" && location.state?.isNew) {
                const recipientEmail = chatInfo?.uEmail || chatInfo?.UEmail;

                console.log("Отправка первого сообщения для нового чата...", {
                    senderId: userId,
                    recipientEmail: recipientEmail
                });

                const response = await axios.post("http://localhost:5000/api/IChat/create-and-send", {
                    senderId: userId,
                    recipientEmail: recipientEmail,
                    messageText: text
                });

                console.log("Ответ после создания чата:", response.data);
                const newChatId = response.data.ichatPublicId;
                navigate(`/chat/${newChatId}`, { replace: true });
            } else {
                console.log("Отправка сообщения в чат:", id);
                await axios.post(`http://localhost:5000/api/IChat/message`, {
                    chatId: id,
                    senderId: userId,
                    text: text
                });
            }
            setText("");
        } catch (err) {
            console.error("Ошибка отправки:", err);
        }
    };

    return (
        <div className="flex flex-col h-[80vh] max-w-2xl mx-auto bg-white shadow-lg rounded-2xl overflow-hidden">
            
            <div className="p-4 border-b bg-gray-50 flex items-center">
                <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white font-bold">
                    {(chatName || "?")[0].toUpperCase()}
                </div>
                <div className="ml-3">
                    <h2 className="font-bold">{chatName || "Загрузка..."}</h2>
                    
                </div>
            </div>

            <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-gray-100">
                {messages.map((m, i) => {
                    if (i === 0) console.log("Пример объекта сообщения в массиве:", m);
                    const msgText = m.text || m.Text;
                    const msgSenderId = m.senderId || m.SenderId;

                    return (
                        <div key={i} className={`flex ${Number(msgSenderId) === userId ? "justify-end" : "justify-start"}`}>
                            <div className={`p-3 rounded-2xl max-w-[80%] ${Number(msgSenderId) === userId ? "bg-blue-600 text-white" : "bg-white text-gray-800 shadow-sm"}`}>
                                {msgText}
                            </div>
                        </div>
                    );
                })}
                {id === "new" && messages.length === 0 && (
                    <p className="text-center text-gray-400 text-sm mt-10">Напишите что-нибудь, чтобы начать общение</p>
                )}
            </div>

            <form onSubmit={sendMessage} className="p-4 border-t bg-white flex gap-2">
                <input
                    type="text"
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    placeholder="Ваше сообщение..."
                    className="flex-1 p-2 border rounded-xl outline-none focus:ring-2 focus:ring-blue-500"
                />
                <button type="submit" className="bg-blue-600 text-white px-6 py-2 rounded-xl hover:bg-blue-700 transition">
                    Отправить
                </button>
            </form>
        </div>
    );
};

export default Chat;