import React from "react";
import Login from "./Auth/Login";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "./MainLayout";
import Register from "./Auth/Registr";

const Application = () => {
    return (
        <BrowserRouter>
            <Routes>
                {/* MainLayout оборачивает дочерние роуты */}
                <Route path="/" element={<MainLayout />}>
                    <Route path="login" element={<Login />} />
                    <Route path="register" element={<Register/>} />
                    {/* Можно добавить страницу по умолчанию */}
                    <Route index element={<Login />} />
                </Route>
            </Routes>
        </BrowserRouter>
    )
}

export default Application;