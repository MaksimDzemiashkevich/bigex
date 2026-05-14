import React from "react";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./MainLayout";
import Login from "./Auth/Login";
import Register from "./Auth/Registr";
import Home from "./Home";
import Chat from "./Communicate/Chat";

const Application = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<MainLayout />}>
                    <Route index element={<Home />} />
                    <Route path="login" element={<Login />} />
                    <Route path="register" element={<Register />} />
                    <Route path="chat/:id" element={<Chat />} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

export default Application;