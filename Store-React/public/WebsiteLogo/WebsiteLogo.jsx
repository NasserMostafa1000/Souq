import React from "react";

const SouqAlBaladLogo = ({ width = 250, height = 250 }) => {
  return (
    <svg
      width={width}
      height={height}
      viewBox="0 0 500 500"
      xmlns="http://www.w3.org/2000/svg"
    >
      <defs>
        {/* تأثير الظل */}
        <filter id="shadow">
          <feDropShadow
            dx="6"
            dy="6"
            stdDeviation="8"
            floodColor="rgba(139,115,85,0.3)"
          />
        </filter>

        {/* تدرجات ثلاثية الأبعاد */}
        <linearGradient id="pyramidFront" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" stopColor="#E3D4B8" />
          <stop offset="100%" stopColor="#A68A64" />
        </linearGradient>

        <linearGradient id="pyramidLeft" x1="0%" y1="0%" x2="100%" y2="50%">
          <stop offset="0%" stopColor="#B8A07A" />
          <stop offset="100%" stopColor="#8B7355" />
        </linearGradient>

        <linearGradient id="pyramidRight" x1="100%" y1="0%" x2="0%" y2="50%">
          <stop offset="0%" stopColor="#C2B280" />
          <stop offset="100%" stopColor="#9C8463" />
        </linearGradient>
      </defs>

      {/* الخلفية */}
      <circle
        cx="250"
        cy="250"
        r="250"
        fill="white"
        stroke="#8B7355"
        strokeWidth="4"
        filter="url(#shadow)"
      />

      {/* الأهرامات ثلاثية الأبعاد */}
      <g transform="translate(50 120)">
        {/* الهرم الأكبر */}
        <g transform="translate(0 0)">
          {/* الواجهة الأمامية */}
          <polygon
            points="80,100 180,20 280,100"
            fill="url(#pyramidFront)"
            stroke="#A68A64"
            strokeWidth="2"
          />

          {/* الجانب الأيسر */}
          <polygon
            points="80,100 180,20 180,100"
            fill="url(#pyramidLeft)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />

          {/* الجانب الأيمن */}
          <polygon
            points="180,20 280,100 180,100"
            fill="url(#pyramidRight)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />
        </g>

        {/* الهرم الأوسط */}
        <g transform="translate(200 -20)">
          <polygon
            points="60,140 130,80 200,140"
            fill="url(#pyramidFront)"
            stroke="#A68A64"
            strokeWidth="2"
          />
          <polygon
            points="60,140 130,80 130,140"
            fill="url(#pyramidLeft)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />
          <polygon
            points="130,80 200,140 130,140"
            fill="url(#pyramidRight)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />
        </g>

        {/* الهرم الأصغر */}
        <g transform="translate(320 40)">
          <polygon
            points="40,160 80,120 120,160"
            fill="url(#pyramidFront)"
            stroke="#A68A64"
            strokeWidth="2"
          />
          <polygon
            points="40,160 80,120 80,160"
            fill="url(#pyramidLeft)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />
          <polygon
            points="80,120 120,160 80,160"
            fill="url(#pyramidRight)"
            stroke="#8B7355"
            strokeWidth="1.5"
          />
        </g>

        {/* الرمال */}
        <path
          d="M-20 180 Q100 200 250 180 T500 160"
          fill="#E3D4B8"
          opacity="0.8"
        />
      </g>

      {/* النصوص */}
      <text
        x="250"
        y="70"
        fontSize="32"
        fontFamily="'Segoe UI', Tahoma"
        fill="#8B7355"
        textAnchor="middle"
        fontWeight="600"
      >
        سوق البلد
      </text>

      <text
        x="250"
        y="430"
        fontSize="26"
        fontFamily="'Segoe UI', Tahoma"
        fill="#8B7355"
        textAnchor="middle"
        letterSpacing="1.5"
      >
        Souq Al Balad
      </text>
    </svg>
  );
};

export default SouqAlBaladLogo;
