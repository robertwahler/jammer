module BasicUnity

  class Version < Thor
    class Read < Thor
      namespace 'version:read'
      include Thor::Actions
      include BasicUnity::BasicUnityHelper

      # adds :quiet, :skip, :pretent, :force
      add_runtime_options!

      desc "number", "display the current version number from Assets/Resources/Version.txt"
      def number
        puts read_version_number
      end

      desc "code", "display the current version code from Assets/Resources/Version.txt"
      def code
        puts read_version_code
      end

      desc "build", "display the current version build from Assets/Resources/Version.txt"
      def build
        puts read_version_build
      end

      desc "string", "display the entire current version string from Assets/Resources/Version.txt"
      def string
        puts read_version_string
      end

      desc "identifier", "display the product identifier used by iTunes and Google Play stores"
      def identifier
        puts read_product_identifier
      end

      desc "sha", "display the sha directly from git"
      def sha
        puts generate_build_number
      end

      desc "unity", "display the unity version used for last build, auto written in post build process"
      def unity
        puts read_unity_version
      end

      private

    end

    class Write < Thor
      namespace 'version:write'
      include Thor::Actions
      include BasicUnity::BasicUnityHelper

      # adds :quiet, :skip, :pretent, :force
      add_runtime_options!

      desc "number <NUMBER>", "write the given version number to Version.txt and ProjectSettings.asset"
      def number(number)
        # version JSON
        write_version_number(number)
        # project settings
        write_project_number(number)
        # always update build automatically
        write_version_build(nil)
      end

      desc "code <CODE>", "write the given code number to Version.txt and iOS plist"
      def code(code)
        # version JSON
        write_version_code(code)
        # project settings
        write_project_code(code)
        # always update build automatically
        write_version_build(nil)
      end

      desc "identifier [IDENTIFIER]", "write the product identifier used by iTunes and Google Play stores"
      def identifier(identifier=nil)
        write_project_identifier(identifier)
      end

      desc "build [SHA]", "write the build to Version.txt, defaults to git SHA"
      def build(sha=nil)
        write_version_build(sha)
      end

      private

    end
  end
end
