#!/bin/bash
set -e

echo "🚀 Configurando entorno .NET MAUI 10..."

# ── 0. Microsoft OpenJDK 21 ───────────────────────────────────────────────────
echo "☕ Instalando Microsoft OpenJDK 21..."

wget -qO /tmp/packages-microsoft-prod.deb \
  "https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb"
sudo dpkg -i /tmp/packages-microsoft-prod.deb
rm /tmp/packages-microsoft-prod.deb

sudo apt-get update -q
sudo apt-get install -y msopenjdk-21

# Exportamos para el uso de este script
export JAVA_HOME=/usr/lib/jvm/msopenjdk-21-amd64
export PATH=$JAVA_HOME/bin:$PATH

# Lo dejamos en bashrc por si se abre una terminal pura (fuera de VS Code)
echo "export JAVA_HOME=/usr/lib/jvm/msopenjdk-21-amd64" >> ~/.bashrc
echo 'export PATH=$JAVA_HOME/bin:$PATH' >> ~/.bashrc

echo "✅ Java instalado: $(java -version 2>&1 | head -1)"

# ── 1. Workloads de MAUI ──────────────────────────────────────────────────────
echo "📦 Instalando workloads de .NET MAUI..."
dotnet workload install maui-android

# ── 2. Android SDK ────────────────────────────────────────────────────────────
if [ ! -d "$ANDROID_SDK_ROOT/cmdline-tools" ]; then
  echo "📱 Descargando Android Command Line Tools..."
  mkdir -p "$ANDROID_SDK_ROOT/cmdline-tools"

  CMDLINE_TOOLS_URL="https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip"
  TMP_ZIP="/tmp/cmdline-tools.zip"

  curl -fsSL "$CMDLINE_TOOLS_URL" -o "$TMP_ZIP"
  unzip -q "$TMP_ZIP" -d "$ANDROID_SDK_ROOT/cmdline-tools"

  mv "$ANDROID_SDK_ROOT/cmdline-tools/cmdline-tools" "$ANDROID_SDK_ROOT/cmdline-tools/latest"
  rm "$TMP_ZIP"

  echo "✅ Command Line Tools instalados"
else
  echo "✅ Android Command Line Tools ya presentes, saltando descarga"
fi

sudo apt-get update
sudo apt-get install -y ca-certificates ca-certificates-java
sudo update-ca-certificates

# ── 3. Aceptar licencias y componentes Android ────────────────────────────────
export PATH="$PATH:$ANDROID_SDK_ROOT/cmdline-tools/latest/bin:$ANDROID_SDK_ROOT/platform-tools:$ANDROID_SDK_ROOT/emulator"

echo "🔧 Aplicando fix para sdkmanager y corrigiendo permisos..."
sudo chown $(whoami):$(whoami) ~/.android
touch ~/.android/repositories.cfg

echo "📝 Aceptando licencias de Android SDK..."
yes | sdkmanager --sdk_root="$ANDROID_SDK_ROOT" --licenses > /dev/null 2>&1 || true

echo "📦 Instalando componentes Android necesarios para MAUI 10 (API 36)..."
sdkmanager --sdk_root="$ANDROID_SDK_ROOT" \
  "platform-tools" \
  "platforms;android-36" \
  "build-tools;36.0.0"

# ── 4. ADB ────────────────────────────────────────────────────────────────────
echo "🔌 Verificando ADB..."
if command -v adb &> /dev/null; then
  echo "✅ ADB disponible: $(adb version | head -1)"
else
  echo "⚠️  ADB no encontrado. Revisa la instalación de platform-tools."
fi

# ── 5. Verificación final ─────────────────────────────────────────────────────
echo ""
echo "🎉 Setup completado. Verificación del entorno:"
echo "   .NET SDK:     $(dotnet --version)"
echo "   Java:         $(java -version 2>&1 | head -1)"
echo "   Android SDK:  $ANDROID_SDK_ROOT"
echo ""
echo "📡 Para WiFi debugging:"
echo "   adb pair IP:PUERTO   (con el código del teléfono)"
echo "   adb connect IP:5555"
echo "   adb devices"
echo ""
echo "🏗️  Para crear un proyecto MAUI nuevo:"
echo "   dotnet new maui -n MiApp"
echo "   cd MiApp"
echo "   dotnet build -t:Run -f net10.0-android"